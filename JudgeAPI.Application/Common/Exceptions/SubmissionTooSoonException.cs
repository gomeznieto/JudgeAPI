namespace JudgeAPI.Application.Common.Exceptions
{
    public class SubmissionTooSoonException(string message) : AppException(message)
    {
    }

}
