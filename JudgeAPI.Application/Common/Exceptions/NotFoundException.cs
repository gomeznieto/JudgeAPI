namespace JudgeAPI.Application.Common.Exceptions
{
    public class NotFoundException(string message) : AppException(message)
    {
    }

}
