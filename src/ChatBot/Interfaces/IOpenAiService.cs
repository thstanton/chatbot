namespace ChatBot.Interfaces;

public interface IOpenAiService
{
    Task<ChatCompletion> TestCompletionAsync();
    Task<string> GetCompletionAsync(string prompt);
    IAsyncEnumerable<string> RespondToMessageAsync(ChatMessage[] messages);
}