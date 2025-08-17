using ChatBot.Extensions;
using Microsoft.Extensions.Configuration;

namespace ChatBot.Services;

public class OrchestratorService(OpenAiService openAiService, ISupabaseRepository repository, IConfiguration configuration)
{
    public ChatContext? CurrentChatContext { get; set; }
    public List<ChatEvent> CurrentChatEvents { get; set; } = [];
    public List<ChatMessage> CurrentChatMessages { get; set; } = [];
    
    public async Task InitialiseChatSession(Guid userId)
    {
        var existingChatContext = await repository.GetChatContextByDate(userId, DateTime.Today);

        if (existingChatContext != null)
        {
            CurrentChatContext = existingChatContext;
            CurrentChatEvents = await repository.GetContextChatEvents(existingChatContext.Id);
            
            CurrentChatMessages.Add(new SystemChatMessage(CurrentChatContext.SystemPrompt));

            foreach (var chatEvent in CurrentChatEvents)
            {
                switch (chatEvent.Sender)
                {
                    case Sender.User:
                        CurrentChatMessages.Add(new UserChatMessage(chatEvent.Content));
                        break;
                    case Sender.Assistant:
                        CurrentChatMessages.Add(new AssistantChatMessage(chatEvent.Content));
                        break;
                    case Sender.System:
                        CurrentChatMessages.Add(new SystemChatMessage(chatEvent.Content));
                        break;
                    case Sender.Tool:
                        CurrentChatMessages.Add(new ToolChatMessage(chatEvent.Content));
                        break;
                    default:
                        continue;
                }
            }
            return;
        }

        var newContext = await InitialiseNewChatContext(userId);
        
        CurrentChatContext = await repository.CreateChatContext(newContext.UserId, newContext.SystemPrompt);
    }

    private async Task<ChatContext> InitialiseNewChatContext(Guid userId)
    {
        var mostRecentChatContext = await repository.GetMostRecentChatContext(userId);
        var user = GetUserFromConfiguration();
        var userProfile = GetUserProfileFromConfiguration();
        var basePrompt = configuration.GetSection("AgentConfig").GetValue<string>("basePrompt")!;
        var chatSummary = await SummariseChatContext(mostRecentChatContext);

        return new ChatContext()
        {
            UserId = userId,
            SystemPrompt = ConstructSystemPrompt(user, userProfile, basePrompt, chatSummary),
        };
    }

    private static string ConstructSystemPrompt(User user, UserProfile userProfile, string basePrompt,
        string? chatSummary)
    {
        var userPrompt =
            $"Your client is {user.DisplayName}, a {userProfile.DateOfBirth.GetAge()} year old {userProfile.Sex.ToString()}." +
            $"Their height is {userProfile.Height}{userProfile.HeightUnit}. " +
            $"Their current goal is {userProfile.Goal}.";

        var prompt = basePrompt;
        
        prompt += userPrompt;

        if (chatSummary != null)
        {
            prompt += $"\nPrevious conversation summary: {chatSummary}";
        }
        
        return prompt;
    }

    private async Task<string?> SummariseChatContext(ChatContext? chatContext)
    {
        if (chatContext == null) return null;
        
        var chatEvents = await repository.GetContextChatEvents(chatContext.Id);
        var chatEventsPromptString = chatEvents.ToPromptString();
        
        var prompt = $"Summarise the following chat events for use in a system prompt: {chatEventsPromptString}";

        return await openAiService.GetCompletionAsync(prompt);
    }

    public async IAsyncEnumerable<string> HandleUserInputAsync(string message)
    {
        
    }

    private UserProfile GetUserProfileFromConfiguration()
    {
        var devUser = configuration.GetSection("DevUser");
        
        var dateOfBirth = DateTime.TryParse(devUser.GetValue<string>("dateOfBirth"), out var birthday) ? birthday : DateTime.MinValue;

        return new UserProfile()
        {
            DateOfBirth = dateOfBirth,
            Height = devUser.GetValue<int>("height"),
            HeightUnit = devUser.GetValue<string>("heightUnit") ?? "cm",
            Sex = (Sex)devUser.GetValue<int>("sex"),
            Goal = devUser.GetValue<string>("goal") ?? string.Empty,
        };
    }

    private User GetUserFromConfiguration()
    {
        var devUser = configuration.GetSection("DevUser");

        return new User()
        {
            DisplayName = devUser.GetValue<string>("displayName") ?? string.Empty,
        };
    }
}