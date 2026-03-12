namespace JudgeAPI.Models.Execution
{
    public class CompilationResult
    {
        public string ExePath { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
        public bool Success => !string.IsNullOrEmpty(ExePath);
        public static CompilationResult Failed() => new CompilationResult();
    }
}
