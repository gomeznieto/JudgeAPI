using JudgeAPI.Application.Features.Submissions.Dtos;

namespace JudgeAPI.Application.Features.Users.Dtos
{
    public class UserPrivateDTO : UserBaseDTO
    {
        public required string UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? University { get; set; }
        public List<SubmissionResponseDTO> Submissions { get; set; } = [];
    }
}
