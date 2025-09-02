using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Models.Submission
{
    public class SubmissionCreateDTO
    {
        [Required]
        [MaxLength(10000, ErrorMessage = "El código es demasiado largo.")]
        public required string Code { get; set; }
        public string Language { get; set; } = "C++";
    }
}
