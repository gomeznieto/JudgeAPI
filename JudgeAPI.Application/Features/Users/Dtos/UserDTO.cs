namespace JudgeAPI.Application.Features.Users.Dtos
{
    public class UserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? University { get; set; }
        public List<string> Roles { get; set; } = [];
        public bool IsActive { get; set; } = true;
    }

}
