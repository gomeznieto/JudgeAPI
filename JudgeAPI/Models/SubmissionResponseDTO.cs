namespace JudgeAPI.Models
{
    public class SubmissionResponseDTO
    {
        public int Id { get; set; }
        public int ProblemId { get; set; }
        public required string UserID { get; set; }
        public string Verdict { get; set; } = "Pending";
        public DateTime SubmissionTime { get; set; }
    }
}
