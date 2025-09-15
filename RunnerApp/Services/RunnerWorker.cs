
using JudgeAPI.Configuration;
using JudgeAPI.Constants;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Models.Execution;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JudgeAPI.Services.Execution
{
    public class RunnerWorker : BackgroundService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IDatabase _redis;
        private readonly RunnerConfig _cfg;
        private readonly IConfiguration _configuration;

        public RunnerWorker(
            AppDbContext appDbContext,
            IConnectionMultiplexer muxer,
            RunnerConfig cfg,
            IConfiguration configuration
        )
        {
            _appDbContext = appDbContext;
            _redis = muxer.GetDatabase();
            _cfg = cfg;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var payload = await _redis.ListLeftPopAsync("submissions");

                if (payload.IsNullOrEmpty)
                {
                    await Task.Delay(400, stoppingToken);
                    continue;
                }

                JobDTO? job = null;

                try
                {
                    job = JsonSerializer.Deserialize<JobDTO>(payload!);
                }
                catch { }

                if (job is null || job!.SubmissionId <= 0) continue;

                try
                {
                    Console.WriteLine($"[Runner] Procesando submission {job.SubmissionId}");
                    await HandleJobAsync(job, stoppingToken);
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"[Runner] Error procesando submission {job?.SubmissionId}: {ex}");
                }

            }
        }

        private async Task HandleJobAsync(JobDTO job, CancellationToken stoppingToken)
        {
            Console.WriteLine($"[Runner] Entró a HandleJobAsync con submission {job.SubmissionId}");

            // Buscamos Submission
            var submission = await _appDbContext.Submissions.FirstOrDefaultAsync(s => s.Id == job.SubmissionId);

            if (submission is null)
                return;

            // Buscamos los casos de prueba
            var testCases = await _appDbContext.TestCases
                .Where(t => t.ProblemId == submission.ProblemId)
                .OrderBy(t => t.Order)
                .ToListAsync();

            // Creamos la carpeta y agregar el archivo cpp
            var guid = Guid.NewGuid().ToString("N");
            var tempDirBase = Path.Combine(Directory.GetCurrentDirectory(), "jobs");

            Directory.CreateDirectory(tempDirBase);

            var jobsRoot = "/app/jobs";

            // TEST
            Console.WriteLine($"[Runner] jobsRoot = {jobsRoot}");


            var tempDir = Path.Combine(jobsRoot, $"job_{submission.Id}_{guid}");

            Directory.CreateDirectory(tempDir);

            // Permisos para escribir
            var dirInfo = new DirectoryInfo(tempDir);
            dirInfo.Attributes &= ~FileAttributes.ReadOnly;

            var chmodResult = Process.Start(new ProcessStartInfo
            {
                FileName = "chmod",
                Arguments = "-R 777 " + tempDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            chmodResult?.WaitForExit();

            var src = Path.Combine(tempDir, "main.cpp");
            File.WriteAllText(src, submission.Code);

            // Compilar
            var std = _configuration["CompilerSettings:CppStandard"] ?? "c++17";
            var flags = _configuration["CompilerSettings:Flags"] ?? "-Wall";
            var compileCmd = $"ls -R /app/jobs && g++ -std=c++17 -Wall /app/jobs/job_{submission.Id}_{guid}/main.cpp -o /app/jobs/job_{submission.Id}_{guid}/a.out";


            Console.WriteLine($"[Runner] wrote source? {File.Exists(src)} size={new FileInfo(src).Length}");
            Console.WriteLine($"[Runner] dir exists: {Directory.Exists(tempDir)}");
            foreach (var file in Directory.GetFiles(tempDir))
            {
                Console.WriteLine($"[Runner] - {file}");
            }

            var compileRes = await RunDockerAsync(tempDir, compileCmd, captureStdErr: true, stoppingToken);

            if (compileRes.ExitCode != 0)
            {
                submission.Verdict = SubmissionVerdicts.CompilationError;
                await _appDbContext.SaveChangesAsync(stoppingToken);
                return;
            }

            // Ejecutar test
            bool allOk = true;
            var toInsert = new List<SubmissionResult>();

            foreach (var tc in testCases)
            {
                var inPath = Path.Combine(tempDir, "input.txt");
                var outPath = Path.Combine(tempDir, "output.txt");

                await File.WriteAllTextAsync(inPath, tc.InputData ?? string.Empty, stoppingToken);
                if (File.Exists(outPath)) File.Delete(outPath);

                var runCmd = $"timeout {_cfg.PerTestTimeoutSeconds}s {tempDir}/a.out < {tempDir}/input.txt > {tempDir}/output.txt";
                var sw = Stopwatch.StartNew();
                var runRes = await RunDockerAsync(tempDir, runCmd, captureStdErr: true, stoppingToken);
                sw.Stop();

                var output = File.Exists(outPath) ? await File.ReadAllTextAsync(outPath, stoppingToken) : string.Empty;

                bool isTle = runRes.ExitCode == 124;
                bool isRe = runRes.ExitCode != 0 && !isTle;

                bool isCorrect = false;

                if (!isTle && !isRe)
                {
                    isCorrect = Normalize(output) == Normalize(tc.ExpectedOutput ?? string.Empty);
                }

                toInsert.Add(new SubmissionResult
                {
                    SubmissionId = submission.Id,
                    TestCaseId = tc.Id,
                    Output = TrimForDb(output),
                    ExecutionTimeMs = sw.ElapsedMilliseconds,
                    IsCorrect = isCorrect
                });

                if (!isCorrect) allOk = false;

                if (isTle)
                {
                    Console.WriteLine($"[Runner] TLE en Submission {submission.Id}, TC {tc.Id}");
                }

                if (isRe)
                {
                    Console.WriteLine($"[Runner] RE (exit {runRes.ExitCode}) en Submission {submission.Id}, TC {tc.Id}");
                }
            }

            if (toInsert.Count > 0)
                _appDbContext.SubmissionResults.AddRange(toInsert);

            submission.Verdict = allOk ? SubmissionVerdicts.Correct : SubmissionVerdicts.Wrong;
            await _appDbContext.SaveChangesAsync(stoppingToken);

            try { Directory.Delete(tempDir, true); } catch { /* ignore */ }
        }

        private static string Normalize(string s)
            => s.Replace("\r\n", "\n").TrimEnd();

        private static string TrimForDb(string s, int max = 2000)
            => s.Length <= max ? s : s.Substring(0, max);

        private async Task<ProcessResult> RunDockerAsync(string hostWorkDir, string innerCmd, bool captureStdErr, CancellationToken stoppingToken)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "docker",
                RedirectStandardOutput = true,
                RedirectStandardError = captureStdErr,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Directory.Exists("/app") ? "/app" : "/"
            };

            var hostJobsDir = Environment.GetEnvironmentVariable("HOST_JOBS_DIR") ?? "./jobs";

            psi.ArgumentList.Add("run");
            psi.ArgumentList.Add("--rm");
            psi.ArgumentList.Add("--network"); 
            psi.ArgumentList.Add("none");
            psi.ArgumentList.Add($"--cpus={_cfg.Cpus}");
            psi.ArgumentList.Add($"--memory={_cfg.MemoryMb}m");
            psi.ArgumentList.Add("--pids-limit"); 
            psi.ArgumentList.Add("256");
            psi.ArgumentList.Add("--read-only");
            psi.ArgumentList.Add("--tmpfs"); psi.ArgumentList.Add("/tmp");
            psi.ArgumentList.Add("-v");
            psi.ArgumentList.Add($"{hostJobsDir}:/app/jobs");
            psi.ArgumentList.Add("--user"); 
            psi.ArgumentList.Add("1000:1000");
            psi.ArgumentList.Add(_cfg.ImageName);
            psi.ArgumentList.Add("/bin/sh");
            psi.ArgumentList.Add("-lc");
            psi.ArgumentList.Add(innerCmd);

            Console.WriteLine("[Runner] cmd: docker " + string.Join(' ', psi.ArgumentList.Select(a => a.Contains(' ') ? $"\"{a}\"" : a)));

            using var p = Process.Start(psi);
            if (p is null) return new ProcessResult { ExitCode = -1 };

            var outTask = p.StandardOutput.ReadToEndAsync();
            var errTask = captureStdErr ? p.StandardError.ReadToEndAsync() : Task.FromResult(string.Empty);

            await p.WaitForExitAsync(stoppingToken);
            var outText = await outTask;
            var errText = await errTask;

            Console.WriteLine($"[docker output]\n{outText}");
            Console.WriteLine($"[docker error]\n{errText}");

            return new ProcessResult
            {
                ExitCode = p.ExitCode,
                StdOut = outText,
                StdErr = errText
            };
        }


    }
}
