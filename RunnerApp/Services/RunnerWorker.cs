
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
            var guid2 = Guid.NewGuid().ToString();
            var tempDir = Path.Combine(Path.GetTempPath(), $"job_{submission.Id}_{guid}");
            Directory.CreateDirectory(tempDir);
            var src = Path.Combine(tempDir, "main.cpp");
            await File.WriteAllTextAsync(src, submission.Code ?? string.Empty, stoppingToken);

            // Compilar
            var std = _configuration["CompilerSettings:CppStandard"] ?? "c++17";
            var flags = _configuration["CompilerSettings:Flags"] ?? "-Wall";
            var compileCmd = $"-std={std} {flags} /work/main.cpp -o /work/a.out";

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

                var runCmd = $"timeout {_cfg.PerTestTimeoutSeconds}s /work/a.out < /work/input.txt > /work/output.txt";
                var sw = Stopwatch.StartNew();
                var runRes = await RunDockerAsync(tempDir, runCmd, captureStdErr: true, stoppingToken);
                sw.Stop();

                var output = File.Exists(outPath) ? await File.ReadAllTextAsync(outPath, stoppingToken) : string.Empty;

                bool isTle = runRes.ExitCode == 124; // 'timeout' devuelve 124
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

            var args = new[]
            {
                "run","--rm",
                "--network","none",
                $"--cpus={_cfg.Cpus}",
                $"--memory={_cfg.MemoryMb}m",
                "--pids-limit","256",
                "--read-only",
                "-v",$"{hostWorkDir}:/work",
                "--user","1000:1000",
                _cfg.ImageName,
                "/bin/sh","-lc", innerCmd
            };

            var workDir = Directory.Exists("/app") ? "/app" : "/";

            var psi = new ProcessStartInfo("docker", string.Join(' ', args))
            {
                RedirectStandardOutput = true,
                RedirectStandardError = captureStdErr,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workDir
            };


            Console.WriteLine("[Runner] WorkingDirectory: " + psi.WorkingDirectory);
            Console.WriteLine("[Runner] docker path: " + Environment.GetEnvironmentVariable("PATH"));
            Console.WriteLine("[Runner] file exists? " + File.Exists("/usr/bin/docker"));
            Console.WriteLine("[Runner] can run docker directly?");

            Process? p;

            try
            {
                p = Process.Start(psi);
                if (p is null)
                {
                    Console.WriteLine("[Runner] ❌ No se pudo iniciar el proceso Docker.");
                    return new ProcessResult { ExitCode = -1 };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Runner] ❌ Excepción lanzada al iniciar Docker: {ex}");
                return new ProcessResult { ExitCode = -1 };
            }

            if (p == null)
{
                Console.WriteLine("[Runner] ❌ No se pudo iniciar el proceso Docker.");
            }

            var stdOutTask = p.StandardOutput.ReadToEndAsync();
            var stdErrTask = captureStdErr ? p.StandardError.ReadToEndAsync() : Task.FromResult(string.Empty);

            await p.WaitForExitAsync(stoppingToken);
            var outText = await stdOutTask;
            var errText = await stdErrTask;

            Console.WriteLine($"[docker output]\n{outText}");
            Console.WriteLine($"[docker error]\n{errText}");

            return new ProcessResult()
            {
                ExitCode = p.ExitCode,
                StdErr = outText,
                StdOut = errText
            };
        }

    }
}
