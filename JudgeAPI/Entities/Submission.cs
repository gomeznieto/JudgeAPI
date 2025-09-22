using JudgeAPI.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JudgeAPI.Entities
{
    public class Submission
    {
        public int Id { get; set; }
        public string UserId { get; set; } = String.Empty;
        [Required]
        public int ProblemId { get; set; }
        [ForeignKey(nameof(ProblemId))]
        public Problem? Problem { get; set; }
        public string? Code { get; set; }
        public string Language { get; set; } = Languages.Cpp;
        public DateTime SubmissionTime { get; set; } = DateTime.UtcNow;
        public string Verdict { get; set; } = SubmissionVerdicts.Pending;
        public ICollection<SubmissionResult>? Results{ get; set; }
    }
}
