namespace JudgeAPI.Models.Unit
{
    public class UnitResponseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Number { get; set; }
        public string? Description { get; set; }
        public bool isActivate { get; set; }
    }
}
