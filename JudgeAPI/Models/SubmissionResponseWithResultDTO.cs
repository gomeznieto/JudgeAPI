namespace JudgeAPI.Models
{
    public class SubmissionResponseWithResultDTO : SubmissionResponseDTO
    {
        public List<SubmissionResultResponseDTO> Results { get; set; } = new();
    }
}
