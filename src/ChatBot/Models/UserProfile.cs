namespace ChatBot.Models;

public class UserProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Height { get; set; }
    public string HeightUnit { get; set; } = "cm";
    public Sex Sex { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Goal { get; set; } = "To gain muscle and lose weight";
    public DateTime CreatedAt { get; set; }
}

public enum Sex
{
    Male,
    Female,
    Other
}