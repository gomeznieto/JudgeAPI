using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JudgeAPI.Entities
{
    public class TestCase
    {
        public int Id { get; set; }
        [Required]
        public int ProblemId { get; set; }
        [ForeignKey(nameof(ProblemId))]
        public Problem? Problem { get; set; }
        [Required]
        public string InputData { get; set; } = String.Empty;
        [Required]
        public string ExpectedOutput { get; set; } = String.Empty;
        public bool IsSample { get; set; } = false;
        public int? Order { get; set; }
    }
}
