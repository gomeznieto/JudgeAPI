namespace JudgeAPI.Application.Features;
using JudgeAPI.Models.Submission;

public class UserPublicDTO : UserBaseDTO {
    public required string UserName { get; set; }
    public List<SubmissionResponseDTO> Submissons { get; set; } = new List<SubmissionResponseDTO>();
}
