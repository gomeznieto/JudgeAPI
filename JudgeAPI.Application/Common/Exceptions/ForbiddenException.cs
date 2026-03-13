namespace JudgeAPI.Application.Common;

public class ForbiddenException : AppException {
    public ForbiddenException(string message) : base(message)
    {
    }
}

