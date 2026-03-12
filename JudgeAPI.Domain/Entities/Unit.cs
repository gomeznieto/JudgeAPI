namespace JudgeAPI.Domain;

public class Unit {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Number { get; set; }
    public string? Description { get; set; }
    public bool isActivate { get; set; } = true;

    public ICollection<Problem>? Problems { get; set; }
}

