
namespace JudgeAPI.Application.Features.CodeExecutor.Dtos
{
    public class CompilationResultDTO
    {
        public string ExePath { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
        public bool Success => !string.IsNullOrEmpty(ExePath);
        public static CompilationResultDTO Failed()
        {
            return new();
        }
    }
}
