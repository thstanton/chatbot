using ChatBot.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SystemPrompt = ChatBot.Models.SystemPrompt;

namespace ChatBot.Services;

public class OrchestratorService(OpenAiService openAiService, ISupabaseRepository repository, IConfiguration configuration, ILogger<OrchestratorService> logger)
{
    private ChatContext? CurrentChatContext { get; set; }
    private List<ChatMessage> CurrentChatMessages { get; set; } = [];
    
    private readonly ILogger<OrchestratorService> _logger = logger;
    
    private readonly OpenAiService _aiService = openAiService;
    
    private readonly IConfiguration _configuration =  configuration;
    
    private readonly ISupabaseRepository _repository = repository;
    
    public async Task InitialiseChatSession(Guid authUserId)
    {
        var user = await _repository.GetUserById(authUserId);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", authUserId);
            throw new UnauthorizedAccessException();
        }
        
        var existingChatContext = await _repository.GetChatContextByDate(user.Id, DateTime.Today);
        
        CurrentChatContext = await GetOrCreateChatContext(user, existingChatContext);
            
        CurrentChatMessages.Add(new SystemChatMessage(CurrentChatContext.SystemPrompt));

        var contextChatEvents = await _repository.GetContextChatEvents(CurrentChatContext.Id);

        if (contextChatEvents.Count != 0)
        {
            var chatMessages = contextChatEvents.ToChatMessages();

            CurrentChatMessages.AddRange(chatMessages);
        }
    }

    private async Task<ChatContext> GetOrCreateChatContext(User user, ChatContext? existingChatContext)
    {
        if (existingChatContext != null)
        {
            return existingChatContext;
        }

        try
        {
            var newContext = await InitialiseNewChatContext(user);
            
            return await _repository.CreateChatContext(newContext.UserId, newContext.SystemPrompt);
        }
        catch (Exception e)
        {
            _logger.LogError("Error creating new chat context: {message}", e.Message);
            throw;
        }
    }

    private async Task<ChatContext> InitialiseNewChatContext(User user)
    {
        var userProfile = await _repository.GetUserProfileById(user.Id);
        var basePrompt = _configuration.GetSection("AgentConfig").GetValue<string>("basePrompt")!;
        
        var mostRecentChatContext = await _repository.GetMostRecentChatContext(user.Id);
        var chatSummary = await SummariseChatContext(mostRecentChatContext);

        var systemPrompt = new SystemPrompt(user, userProfile, basePrompt, chatSummary).Message;
        
        return await _repository.CreateChatContext(user.Id, systemPrompt);
    }

    private async Task<string?> SummariseChatContext(ChatContext? chatContext)
    {
        if (chatContext == null) return null;
        
        var chatEvents = await _repository.GetContextChatEvents(chatContext.Id);

        if (chatEvents.Count == 0) return "No chats yet";
        
        var chatEventsPromptString = chatEvents.ToPromptString();
        
        var prompt = $"Summarise the following chat events for use in a system prompt: {chatEventsPromptString}";

        return await _aiService.GetCompletionAsync(prompt);
    }

    public async IAsyncEnumerable<string> HandleAssistantResponse()
    {
        if (CurrentChatContext == null)
        {
            _logger.LogError("Current Chat Context was not initialised");
            yield break;
        }
        
        var messages = CurrentChatMessages.ToArray();

        var response = _aiService.RespondToMessageAsync(messages);
        
        var completedResponse = new StringBuilder();

        await foreach (var chunk in response)
        {
            completedResponse.Append(chunk);
            yield return chunk;
        }
        
        CurrentChatMessages.Add(new AssistantChatMessage(completedResponse.ToString()));
        
        await _repository.AddChatEvent(new ChatEvent()
        {
            ChatContextId = CurrentChatContext.Id,
            Agent = AgentId.Orchestrator,
            Sender = Sender.Assistant,
            Content = completedResponse.ToString(),
        });
    }

    public async Task<ChatEvent?> HandleUserInput(string message)
    {
        if (CurrentChatContext == null)
        {
            _logger.LogError("Current Chat Context was not initialised");
            return null;
        }
        
        CurrentChatMessages.Add(new UserChatMessage(message));

        return await _repository.AddChatEvent(new ChatEvent()
        {
            ChatContextId = CurrentChatContext.Id,
            Agent = AgentId.Orchestrator,
            Sender = Sender.User,
            Content = message,
        });
    }
}