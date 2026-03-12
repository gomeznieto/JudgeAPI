using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Entities
{
    public class Problem
    {
        public int Id { get; set; }
        [Required]
        public int UnitId { get; set; }
        public Unit? Unit { get; set; }
        public string? Title { get; set; }
        [Required]
        public required string Description { get; set; }
        public string? InputDescription { get; set; }
        public string? OutputDescription { get; set; }
        public string? MinConstraint { get; set; }
        public string? MaxConstraint { get; set; }
        public string? ExampleInput { get; set; }
        public string? ExampleOutput { get; set; }
        public bool isMandatory { get; set; } = false;
        public bool isActivate { get; set; } = true;
    }
}
