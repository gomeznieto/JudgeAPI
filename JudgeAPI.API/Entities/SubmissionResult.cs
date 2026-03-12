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
    public string? Output { get; set; }
    public bool IsCorrect { get; set; }
    public long? ExecutionTimeMs { get; set; }
    public bool IsExecuted {get; set;}
    public bool IsTle { get; set; }
    public bool IsMle { get; set; }
    public bool IsRe { get; set; }
    public int? ExitCode { get; set; }
    public string? ErrorOutput { get; set; }
  }
}
