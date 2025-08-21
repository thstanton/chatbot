namespace ChatBot.Interfaces;

public interface IOrchestratorService
{
    Task<string?> SummariseChatContext(List<ChatEvent> chatEvents);
    IAsyncEnumerable<string> HandleAssistantResponse(ChatMessage[] chatMessages);
}