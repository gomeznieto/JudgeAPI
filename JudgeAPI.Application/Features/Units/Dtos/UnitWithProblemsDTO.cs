using JudgeAPI.Models.Problem;

namespace JudgeAPI.Models.Unit
{
    public class UnitWithProblemsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Number { get; set; }
        public string? Description { get; set; }

        public List<ProblemResponseDTO> Problems { get; set; } = [];
    }
}
