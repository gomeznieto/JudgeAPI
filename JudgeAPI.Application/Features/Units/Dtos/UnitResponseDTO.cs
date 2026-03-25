namespace JudgeAPI.Application.Features.Units.Dtos
{
    public class UnitResponseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Number { get; set; }
        public string? Description { get; set; }
        public bool IsActivate { get; set; }
    }
}
