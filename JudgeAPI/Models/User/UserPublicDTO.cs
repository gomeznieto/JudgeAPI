using JudgeAPI.Models.Submission;

namespace JudgeAPI.Models.User
{
    public class UserPublicDTO : UserBaseDTO
    {
        public required string UserName { get; set; }
        public List<SubmissionResponseDTO> Submissons { get; set; } = new List<SubmissionResponseDTO>();
    }
}
