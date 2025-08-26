namespace JudgeAPI.Models
{
    public class SubmissionResultResponseDTO
    {
        public int TestCaseId { get; set; }
        public string Input { get; set; } = string.Empty;
        public string ExpectedOutput { get; set; } = string.Empty;
        public string? Output { get; set; }
        public bool isCorrect{ get; set; } = true;
        public int? ExecutionTimeMs { get; set; }
    }
}
