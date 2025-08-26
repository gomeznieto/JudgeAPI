using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Models
{
    public class SubmissionCreate
    {
        [Required]
        public int ProblemId { get; set; }
        [Required]
        [MaxLength(10000, ErrorMessage = "El código es demasiado largo.")]
        public required string Code { get; set; }
        public string Language { get; set; } = "C++";
    }
}
