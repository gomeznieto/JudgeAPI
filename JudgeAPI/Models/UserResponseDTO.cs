namespace JudgeAPI.Models
{
    public class UserResponseDTO
    {
        public required string UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? University { get; set; }
    }
}
