namespace ChatBot.Models;

public class User
{
    public Guid Id { get; set; }
    public Guid AuthUser { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}