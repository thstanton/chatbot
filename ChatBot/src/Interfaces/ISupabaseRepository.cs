namespace ChatBot.Interfaces;

public interface ISupabaseRepository
{
    Task<ChatContext> CreateChatContext(Guid userId, string systemPrompt);
    Task<ChatContext?> GetChatContextByDate(Guid userId, DateTime date);
    Task<ChatContext?> GetMostRecentChatContext(Guid userId);
    Task<ChatEvent> AddChatEvent(ChatEvent chatEvent);
    Task<List<ChatEvent>> GetContextChatEvents(Guid contextId);
}