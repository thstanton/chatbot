namespace ChatBot.Models;

public class ChatContext
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string SystemPrompt { get; set; } = string.Empty;
}