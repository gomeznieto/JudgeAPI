namespace JudgeAPI.Application.Common;

public class NotFoundException : AppException {
    public NotFoundException(string message) : base (message) { }
}

