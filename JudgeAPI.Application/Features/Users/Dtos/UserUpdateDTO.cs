namespace JudgeAPI.Application.Features;
using System.ComponentModel.DataAnnotations;

public class UserUpdateDTO {
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? University { get; set; }
}

