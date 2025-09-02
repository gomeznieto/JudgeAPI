namespace JudgeAPI.Models.Submission
{
    public class SubmissionResponseWithResultDTO : SubmissionResponseDTO
    {
        public List<SubmissionResultResponseDTO> Results { get; set; } = new();
    }
}
