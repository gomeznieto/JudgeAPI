using System.Diagnostics;
using JudgeAPI.Application.Features.CodeExecutor.Dtos;
using JudgeAPI.Application.Features.CodeExecutor.Interfaces;
using JudgeAPI.Domain.Entities;

namespace JudgeAPI.Application.Features.CodeExecutor.Services
{
    public class BasicExecutorService : ICodeExecutorService
    {
        public async Task<ExecutionResultDTO> ExecuteAsync(int submissionId, TestCase test, CompilationResultDTO result)
        {

            using Process exeProcess = new();
            var stopwatch = Stopwatch.StartNew();

            exeProcess.StartInfo.UseShellExecute = false;
            exeProcess.StartInfo.FileName = result.ExePath;
            exeProcess.StartInfo.CreateNoWindow = true;
            exeProcess.StartInfo.RedirectStandardError = true;
            exeProcess.StartInfo.RedirectStandardOutput = true;
            exeProcess.StartInfo.RedirectStandardInput = true;
            _ = exeProcess.Start();

            await exeProcess.StandardInput.WriteAsync(test.InputData + "\n");
            exeProcess.StandardInput.Close();

            string output = await exeProcess.StandardOutput.ReadToEndAsync();
            string error = await exeProcess.StandardError.ReadToEndAsync();

            bool exited = exeProcess.WaitForExit(2000);

            stopwatch.Stop();

            if (!exited)
            {
                exeProcess.Kill(entireProcessTree: true);

                return new ExecutionResultDTO
                {
                    Output = "",
                    ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                    TimedOut = true,
                    IsCorrect = false,
                    Error = "Time Limit Exceeded"
                };
            }

            return new ExecutionResultDTO
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
