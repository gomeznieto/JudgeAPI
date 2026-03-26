namespace JudgeAPI.Application.Common.Exceptions
{
    public class ForbiddenException(string message) : AppException(message)
    {
    }

}
