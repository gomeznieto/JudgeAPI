using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Application.Features.TestCases.Dtos
{
    public class TestCaseCreateDTO
    {
        [Required]
        public required string InputData { get; set; }
        [Required]
        public required string ExpectedOutput { get; set; }
        public bool? IsSample { get; set; }
        public int? Order { get; set; }
    }
}
