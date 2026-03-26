using JudgeAPI.Domain.Constants;

namespace JudgeAPI.Application.Features.Submissions.Dtos
{
    public class SubmissionResponseDTO
    {
        public int Id { get; set; }
        public int ProblemId { get; set; }
        public required string UserID { get; set; }
        public string Verdict { get; set; } = SubmissionVerdicts.Pending;
        public DateTime SubmissionTime { get; set; }
        public string Language { get; set; } = Languages.Cpp;
        public string Code { get; set; } = string.Empty;
        public string? CompileError { get; set; }
        public int? CompileExitCode { get; set; }
        public List<SubmissionResultResponseDTO> Results { get; set; } = [];
    }

}
