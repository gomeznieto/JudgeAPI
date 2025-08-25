using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Models
{
    public class ProblemUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        public int UnitId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? InputDescription { get; set; }
        public string? OutputDescription { get; set; }
        public string? MinConstraint { get; set; }
        public string? MaxConstraint { get; set; }
        public bool isActivate { get; set; }
        public bool isMandatory { get; set; }
    }
}
