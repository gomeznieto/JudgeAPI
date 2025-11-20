namespace JudgeAPI.Models.Problem
{
    public class ProblemResponseDTO
    {
        public int Id { get; set; }
        public int UnitId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? InputDescription { get; set; }
        public string? OutputDescription { get; set; }
        public string? MinConstraint { get; set; }
        public string? MaxConstraint { get; set; }
        public bool isMandatory { get; set; } = false;
        public bool isActivate { get; set; }
    }
}
