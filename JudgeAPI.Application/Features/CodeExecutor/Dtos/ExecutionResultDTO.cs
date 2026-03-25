namespace JudgeAPI.Application.Features.CodeExecutor.Dtos
{
    public class ExecutionResultDTO
    {
        public int TestCaseId { get; set; }
        public string Output { get; set; } = string.Empty;
        public long ExecutionTimeMs { get; set; }
        public bool IsCorrect { get; set; }
        public bool TimedOut { get; set; }
        public string? Error { get; set; }

    }
}
