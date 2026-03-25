namespace JudgeAPI.Domain.Entities
{
    public class TestCase
    {
        public int Id { get; set; }
        public string InputData { get; set; } = string.Empty;
        public string ExpectedOutput { get; set; } = string.Empty;
        public bool IsSample { get; set; }
        public int? Order { get; set; }

        public int ProblemId { get; set; }
        public Problem? Problem { get; set; }
    }

}
