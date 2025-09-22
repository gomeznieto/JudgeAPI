using JudgeAPI.Models.Submission;

namespace JudgeAPI.Models.User
{
    public class UserAdminDTO : UserBaseDTO
    {
        public required string UserName { get; set; }
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = [];
        public List<SubmissionResponseDTO> Submissons { get; set; } = new List<SubmissionResponseDTO>();
    }
}
