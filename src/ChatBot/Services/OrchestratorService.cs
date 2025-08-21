namespace ChatBot.Services;

public class OrchestratorService(IOpenAiService openAiService, ILogger<OrchestratorService> logger)
    : IOrchestratorService
{
    private readonly IOpenAiService _aiService = openAiService;
    private readonly ILogger<OrchestratorService> _logger = logger;

    public async Task<string?> SummariseChatContext(List<ChatEvent> chatEvents)
    {
        if (chatEvents.Count == 0) return "No chats yet";

        var chatEventsPromptString = chatEvents.ToPromptString();

        var prompt = $"Summarise the following chat events for use in a system prompt: {chatEventsPromptString}";

        return await _aiService.GetCompletionAsync(prompt);
    }

    public async IAsyncEnumerable<string> HandleAssistantResponse(ChatMessage[] chatMessages)
    {
        var response = _aiService.RespondToMessageAsync(chatMessages);

        await foreach (var chunk in response) yield return chunk;
    }
}