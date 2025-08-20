namespace ChatBot.Services;

public class ChatSessionService(ISupabaseRepository repository,
    IConfiguration configuration,
    ILogger<ChatSessionService> logger,
    IOrchestratorService orchestratorService)
{
    private ChatContext? CurrentChatContext { get; set; }
    private List<ChatMessage> CurrentChatMessages { get; set; } = [];

    private readonly ILogger<ChatSessionService> _logger = logger;

    private readonly IConfiguration _configuration = configuration;

    private readonly ISupabaseRepository _repository = repository;

    private readonly IOrchestratorService _orchestratorService = orchestratorService;

    public async Task InitialiseChatSession(Guid authUserId)
    {
        var user = await _repository.GetUserById(authUserId);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", authUserId);
            throw new UnauthorizedAccessException();
        }

        var existingChatContext = await _repository.GetChatContextByDate(user.Id, DateTime.UtcNow);

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
        var mostRecentChats = mostRecentChatContext != null ? await _repository.GetContextChatEvents(mostRecentChatContext.Id) : [];
        var chatSummary = await _orchestratorService.SummariseChatContext(mostRecentChats);

        var systemPrompt = new SystemPrompt(user, userProfile, basePrompt, chatSummary).Message;

        return await _repository.CreateChatContext(user.Id, systemPrompt);
    }

    public async IAsyncEnumerable<string> HandleAssistantResponse()
    {
        if (CurrentChatContext == null)
        {
            _logger.LogError("Current Chat Context was not initialised");
            yield break;
        }

        var messages = CurrentChatMessages.ToArray();

        var response = _orchestratorService.HandleAssistantResponse(messages);

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