using JudgeAPI.Entities;

namespace JudgeAPI.Models.Execution
{
    public class ExecutionResult
    {
        public string Output { get; set; } = string.Empty;
        public long ExecutionTimeMs { get; set; }
        public bool IsCorrect { get; set; }
        public bool TimedOut { get; set; }
        public string? Error { get; set; }

    }
}
