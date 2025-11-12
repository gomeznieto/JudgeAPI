namespace JudgeAPI.Models.User
{
    public class UserBaseDTO
    {
        public string Token { get; set; } = String.Empty;
        public string UserId { get; set; } = String.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
