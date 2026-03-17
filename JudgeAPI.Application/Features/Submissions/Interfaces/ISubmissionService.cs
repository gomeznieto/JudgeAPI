namespace JudgeAPI.Application.Features;

public interface ISubmissionService {
    Task<bool> AnalyzeSubmissionAsync(int id);
    Task<SubmissionResponseDTO> CreateSubmissionAsync(string userId, int problemId, SubmissionCreateDTO submissionCreateDTO);
    Task<SubmissionResponseDTO> GetSubmissionAsync(int id);
}

