using JudgeAPI.Constants;

namespace JudgeAPI.Models.Submission
{
    public class SubmissionResultResponseDTO
    {
        public int TestCaseId { get; set; }
        public string Input { get; set; } = string.Empty;
        public string ExpectedOutput { get; set; } = string.Empty;
        public string? Output { get; set; }
        public string Language { get; set; } = Languages.Cpp;
        public bool isCorrect{ get; set; } = true;
        public int? ExecutionTimeMs { get; set; }
        public bool IsExecuted {get; set;}
        public bool IsTle { get; set; }
        public int? ExitCode { get; set; }
        public string? ErrorOutput { get; set; }
    }
}
