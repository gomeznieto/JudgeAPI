namespace JudgeAPI.Domain;

public class Submission
{
    public int Id { get; set; }
    public string UserId { get; set; } = String.Empty;

    public string? Code { get; set; }
    public string Language { get; set; } = Languages.Cpp;
    public DateTime SubmissionTime { get; set; } = DateTime.UtcNow;

    public string Verdict { get; set; } = SubmissionVerdicts.Pending;

    public string? CompileError { get; set; }
    public int? CompileExitCode { get; set; }

    public int ProblemId { get; set; }
    public Problem? Problem { get; set; }

    public ICollection<SubmissionResult>? Results{ get; set; }
}

