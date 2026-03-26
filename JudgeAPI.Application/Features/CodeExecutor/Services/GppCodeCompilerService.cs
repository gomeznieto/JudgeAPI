using System.Diagnostics;
using JudgeAPI.Application.Common.Configuration;
using JudgeAPI.Application.Features.CodeExecutor.Dtos;
using JudgeAPI.Application.Features.CodeExecutor.Interfaces;
using Microsoft.Extensions.Options;

namespace JudgeAPI.Application.Features.CodeExecutor.Services
{
    public class GppCodeCompilerService(
            IOptions<SubmissionOptions> submissionOptions,
            IOptions<CompilerOptions> compilerOptions
            ) : ICodeCompilerService
    {
        private readonly SubmissionOptions _submissionOptions = submissionOptions.Value;
        private readonly CompilerOptions _compilerOptions = compilerOptions.Value;


        public async Task<CompilationResultDTO> CompileAsync(string code, int submissionId)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return CompilationResultDTO.Failed();
            }

            string tempFolder = _submissionOptions.Temp ?? string.Empty;
            string basePath = AppContext.BaseDirectory;
            string docPath = Path.Combine(basePath, tempFolder);

            // Crear carpeta si no existe
            if (!Directory.Exists(docPath))
            {
                _ = Directory.CreateDirectory(docPath);
            }

            // Guardar archivo fuente
            string filePath = Path.Combine(docPath, $"{submissionId}.cpp");
            await File.WriteAllTextAsync(filePath, code);

            // Archivo ejecutable
            string exePath = Path.Combine(docPath, $"{submissionId}.exe");

            // Configuración para compilar
            string std = _compilerOptions.CppStandard ?? "c++17";
            string flags = _compilerOptions.Flags ?? "-Wall";

            // Compiler
            using Process cppProcess = new();

            try
            {
                cppProcess.StartInfo.UseShellExecute = false;
                cppProcess.StartInfo.FileName = "g++";
                cppProcess.StartInfo.Arguments = $"-std={std} {flags} \"{filePath}\" -o \"{exePath}\"";
                cppProcess.StartInfo.RedirectStandardError = true;
                cppProcess.StartInfo.CreateNoWindow = true;
                cppProcess.StartInfo.WorkingDirectory = docPath;
                _ = cppProcess.Start();

                string error = await cppProcess.StandardError.ReadToEndAsync();
                cppProcess.WaitForExit();

                if (!string.IsNullOrWhiteSpace(error))
                {
                    File.Delete(filePath);
                    return CompilationResultDTO.Failed();
                }

            }
            catch (Exception)
            {
                File.Delete(filePath);
                throw;
            }

            File.Delete(filePath);

            return new CompilationResultDTO()
            {
                ExePath = exePath,
                SourcePath = filePath
            };
        }
    }
}
