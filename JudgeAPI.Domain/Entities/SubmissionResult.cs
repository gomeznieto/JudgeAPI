namespace JudgeAPI.Domain;

public class SubmissionResult {
    public int Id { get; set; }
    public string? Output { get; set; }
    public bool IsCorrect { get; set; }
    public long? ExecutionTimeMs { get; set; }
    public bool IsExecuted {get; set;}
    public bool IsTle { get; set; }
    public bool IsMle { get; set; }
    public bool IsRe { get; set; }
    public int? ExitCode { get; set; }
    public string? ErrorOutput { get; set; }

    public int TestCaseId { get; set; }
    public TestCase? TestCase { get; set; }

    public int SubmissionId { get; set; }
    public Submission? Submission { get; set; }
}

