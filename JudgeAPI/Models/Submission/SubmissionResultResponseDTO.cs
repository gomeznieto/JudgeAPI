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
    }
}
