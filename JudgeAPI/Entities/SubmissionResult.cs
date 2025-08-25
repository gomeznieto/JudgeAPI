using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JudgeAPI.Entities
{
    public class SubmissionResult
    {
        public int Id { get; set; }
        [Required]
        public int SubmissionId { get; set; }
        [ForeignKey(nameof(SubmissionId))]
        public Submission? Submission { get; set; }
        [Required]
        public int TestCaseId { get; set; }
        [ForeignKey(nameof(TestCaseId))]
        public TestCase? TestCase { get; set; }
        [Required]
        public string? Output { get; set; }
        public bool IsCorrect { get; set; }
        public int? ExecutionTimeMs { get; set; }
    }
}
