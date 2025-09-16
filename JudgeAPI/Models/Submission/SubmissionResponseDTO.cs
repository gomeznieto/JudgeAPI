using JudgeAPI.Constants;

namespace JudgeAPI.Models.Submission
{
    public class SubmissionResponseDTO
    {
        public int Id { get; set; }
        public int ProblemId { get; set; }
        public required string UserID { get; set; }
        public string Verdict { get; set; } = SubmissionVerdicts.Pending;
        public DateTime SubmissionTime { get; set; }
        public string Language { get; set; } = Languages.Cpp;
    }
}
