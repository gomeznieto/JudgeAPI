namespace JudgeAPI.Domain.Entities
{
    public class Unit
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Number { get; set; }
        public string? Description { get; set; }
        public bool IsActivate { get; set; } = true;

        public ICollection<Problem>? Problems { get; set; }
    }

}
