namespace JudgeAPI.Domain.Entities
{
    public class Problem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public required string Description { get; set; }
        public string? InputDescription { get; set; }
        public string? OutputDescription { get; set; }
        public string? MinConstraint { get; set; }
        public string? MaxConstraint { get; set; }
        public string? ExampleInput { get; set; }
        public string? ExampleOutput { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsActivate { get; set; } = true;

        public int UnitId { get; set; }
        public Unit? Unit { get; set; }
    }
}
