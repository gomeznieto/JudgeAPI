using JudgeAPI.Models.Submission;

namespace JudgeAPI.Models.User
{
    public class UserPrivateDTO : UserResponseDTO
    {
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? University { get; set; }
    }
}
