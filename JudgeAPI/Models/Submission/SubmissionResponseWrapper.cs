namespace JudgeAPI.Models.Submission
{
    public class SubmissionResponseWrapper
    {
        public bool IsPending => Verdict == "Pending";

        public SubmissionResponseDTO? Summary { get; set; }
        public SubmissionResponseWithResultDTO? Results{ get; set; }

        public string Verdict { get; set; } = "Pending";
    }
}
