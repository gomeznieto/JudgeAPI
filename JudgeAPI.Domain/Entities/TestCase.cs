namespace JudgeAPI.Domain;

public class TestCase {
    public int Id { get; set; }
    public string InputData { get; set; } = String.Empty;
    public string ExpectedOutput { get; set; } = String.Empty;
    public bool IsSample { get; set; } = false;
    public int? Order { get; set; }

    public int ProblemId { get; set; }
    public Problem? Problem { get; set; }
}

