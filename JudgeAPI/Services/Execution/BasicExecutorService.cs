using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Models.Execution;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JudgeAPI.Services.Execution
{
    public class BasicExecutorService : ICodeExecutorService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger _logger;

        public BasicExecutorService(
            AppDbContext appDbContext,
            ILogger<BasicExecutorService> logger
        )
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<ExecutionResult> ExecuteAsync(int submissionId, TestCase test, CompilationResult result)
        {

            using (Process exeProcess = new Process())
            {
                var stopwatch = Stopwatch.StartNew();

                exeProcess.StartInfo.UseShellExecute = false;
                exeProcess.StartInfo.FileName = result.ExePath;
                exeProcess.StartInfo.CreateNoWindow = true;
                exeProcess.StartInfo.RedirectStandardError = true;
                exeProcess.StartInfo.RedirectStandardOutput = true;
                exeProcess.StartInfo.RedirectStandardInput = true;
                exeProcess.Start();

                await exeProcess.StandardInput.WriteAsync(test.InputData + "\n");
                exeProcess.StandardInput.Close();

                string output = await exeProcess.StandardOutput.ReadToEndAsync();
                var error = await exeProcess.StandardError.ReadToEndAsync();

                var exited = exeProcess.WaitForExit(2000);

                stopwatch.Stop();

                if (!exited)
                {
                    exeProcess.Kill(entireProcessTree: true);

                    return new ExecutionResult
                    {
                        Output = "",
                        ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                        TimedOut = true,
                        IsCorrect = false,
                        Error = "Time Limit Exceeded"
                    };
                }

                return new ExecutionResult
                {
                    Output = output.Trim(),
                    ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                    TimedOut = false,
                    IsCorrect = output.Trim() == test.ExpectedOutput.Trim(),
                    Error = string.IsNullOrWhiteSpace(error) ? null : error

                };

            }

        }
    }
}
