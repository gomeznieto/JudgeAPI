using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Models.Unit
{
    public class UnitCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
        [Required]
        [Range(1, 100)]
        public int Number { get; set; }
        [MaxLength(300)]
        public string? Description { get; set; }
    }
}
