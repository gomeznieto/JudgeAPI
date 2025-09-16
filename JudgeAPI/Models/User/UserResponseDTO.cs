using JudgeAPI.Models.Submission;

namespace JudgeAPI.Models.User
{
    public class UserResponseDTO
    {
        public required string UserName { get; set; }

        public List<SubmissionResponseDTO> Submissons { get; set; } = new List<SubmissionResponseDTO>();
    }
}
