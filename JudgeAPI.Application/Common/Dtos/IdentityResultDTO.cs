namespace JudgeAPI.Application.Common;

public class IdentityResultDTO{
    public bool Succeeded { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}
