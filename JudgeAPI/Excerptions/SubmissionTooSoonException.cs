namespace JudgeAPI.Excerptions {

  public class SubmissionTooSoonException : AppException
  {
    public SubmissionTooSoonException(string message) : base(message)
    {

    }
  }
}
