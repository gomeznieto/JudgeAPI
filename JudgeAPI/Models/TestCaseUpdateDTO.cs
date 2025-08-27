using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Models
{
    public class TestCaseUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public required string InputData { get; set; }
        [Required]
        public required string ExpectedOutput { get; set; }
        public bool? IsSample { get; set; }
        public int? Order { get; set; }
    }
}
