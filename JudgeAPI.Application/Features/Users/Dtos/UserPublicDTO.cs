using JudgeAPI.Application.Features.Submissions.Dtos;

namespace JudgeAPI.Application.Features.Users.Dtos
{
    public class UserPublicDTO : UserBaseDTO
    {
        public required string UserName { get; set; }
        public List<SubmissionResponseDTO> Submissons { get; set; } = [];
    }
}
