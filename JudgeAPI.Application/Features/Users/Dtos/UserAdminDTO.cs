using JudgeAPI.Application.Features.Submissions.Dtos;

namespace JudgeAPI.Application.Features.Users.Dtos
{
    public class UserAdminDTO : UserBaseDTO
    {
        public required string UserName { get; set; }
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<SubmissionResponseDTO> Submissons { get; set; } = [];
    }
}
