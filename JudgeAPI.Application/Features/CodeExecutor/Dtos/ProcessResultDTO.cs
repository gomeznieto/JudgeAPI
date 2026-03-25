namespace JudgeAPI.Application.Features.CodeExecutor.Dtos
{
    public class ProcessResultDTO
    {
        public int ExitCode { get; set; }
        public string StdOut { get; set; } = string.Empty;
        public string StdErr { get; set; } = string.Empty;
    }
}
