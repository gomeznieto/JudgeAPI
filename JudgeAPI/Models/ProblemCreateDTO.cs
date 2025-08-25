using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Models
{
    public class ProblemCreateDTO
    {
        public int UnitId { get; set; }
        [Required]
        public required string Title { get; set; } 
        [Required]
        public required string Description { get; set; }
        public string? InputDescription { get; set; }
        public string? OutputDescription { get; set; }
        public string? MinConstraint { get; set; }
        public string? MaxConstraint { get; set; }
        public bool isMandatory { get; set; } = false;
    }
}
