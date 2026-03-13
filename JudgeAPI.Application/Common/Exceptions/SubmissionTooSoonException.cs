namespace JudgeAPI.Application.Common;

public class SubmissionTooSoonException : AppException {
    public SubmissionTooSoonException(string message) : base(message)
    {

    }
}

