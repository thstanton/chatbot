namespace ChatBot.Models;

public class ChatEvent
{
    public Guid Id { get; set; }
    public Guid ChatContextId { get; set; }
    public AgentId Agent { get; set; }
    public Sender Sender { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public enum Sender
{
    User, 
    Assistant, 
    System, 
    Tool
}

public enum AgentId
{
    Trainer,
    Nutritionist,
    Orchestrator
}
    