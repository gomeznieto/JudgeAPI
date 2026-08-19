namespace JudgeAPI.Application.Common.Dtos
{
    public class IdentityResultDTO
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public string? UserId { get; set; }
    }
}
