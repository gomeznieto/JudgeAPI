using System.ComponentModel.DataAnnotations;
using JudgeAPI.Domain;

namespace JudgeAPI.Application.Features.Submissions.Dtos
{
    public class SubmissionCreateDTO
    {
        [Required]
        [MaxLength(10000, ErrorMessage = "El código es demasiado largo.")]
        public required string Code { get; set; }
        public string Language { get; set; } = Languages.Cpp;
    }
}
