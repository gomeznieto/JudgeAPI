using JudgeAPI.Entities;
using JudgeAPI.Models.Execution;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JudgeAPI.Services.Execution
{
    public class GppCodeCompilerService : ICodeCompilerService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GppCodeCompilerService> _logger;

        public GppCodeCompilerService(
            IConfiguration configuration,
            ILogger<GppCodeCompilerService> logger
        )
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<CompilationResult> CompileAsync(string code, int submissionId)
        {
            if (string.IsNullOrWhiteSpace(code))
                return CompilationResult.Failed();

            string tempFolder = _configuration["SubmissionPaths:Temp"] ?? string.Empty;
            string basePath = AppContext.BaseDirectory;
            string docPath = Path.Combine(basePath, tempFolder);

            // Crear carpeta si no existe
            if (!Directory.Exists(docPath))
                Directory.CreateDirectory(docPath);

            // Guardar archivo fuente
            string filePath = Path.Combine(docPath, $"{submissionId}.cpp");
            await File.WriteAllTextAsync(filePath, code);

            // Archivo ejecutable
            string exePath = Path.Combine(docPath, $"{submissionId}.exe");

            // Configuración para compilar
            var std = _configuration["CompilerSettings:CppStandard"] ?? "c++17";
            var flags = _configuration["CompilerSettings:Flags"] ?? "-Wall";

            // Compiler
            using (Process cppProcess = new Process())
            {
                try
                {
                    cppProcess.StartInfo.UseShellExecute = false;
                    cppProcess.StartInfo.FileName = "g++";
                    cppProcess.StartInfo.Arguments = $"-std={std} {flags} \"{filePath}\" -o \"{exePath}\"";
                    cppProcess.StartInfo.RedirectStandardError = true;
                    cppProcess.StartInfo.CreateNoWindow = true;
                    cppProcess.StartInfo.WorkingDirectory = docPath;
                    cppProcess.Start();

                    string error = await cppProcess.StandardError.ReadToEndAsync();
                    cppProcess.WaitForExit();

                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        _logger.LogError("Error de compilación: {error}", error);
                        File.Delete(filePath);
                        return CompilationResult.Failed();
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error de compilación: {ex.Message}");
                    File.Delete(filePath);
                    throw;
                }
            }

            File.Delete(filePath);

            return new CompilationResult()
            {
                ExePath = exePath,
                SourcePath = filePath
            };
        }
    }
}
