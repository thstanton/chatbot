using ChatBot.Contexts;

namespace ChatBot.Repositories;

public class SupabaseRepository(SupabaseContext db, ILogger<SupabaseRepository> logger) : ISupabaseRepository
{
    private readonly ILogger<SupabaseRepository> _logger = logger;

    public async Task<ChatContext> CreateChatContext(Guid userId, string systemPrompt)
    {
        var result = await db.ChatContexts.AddAsync(new ChatContext
        {
            UserId = userId,
            SystemPrompt = systemPrompt,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<ChatContext?> GetChatContextByDate(Guid userId, DateTime date)
    {
        _logger.LogInformation($"Method entry - date: {date:yyyy-MM-dd HH:mm:ss} (Kind: {date.Kind})");

        var start = date.Date;
        _logger.LogInformation($"After assignment - start: {start:yyyy-MM-dd HH:mm:ss} (Kind: {start.Kind})");
        _logger.LogInformation($"Are they equal? {date == start}");

        var end = start.AddDays(1);

        return await db.ChatContexts.FirstOrDefaultAsync(x => x.UserId == userId
                                                              && x.CreatedAt <= end
                                                              && x.CreatedAt >= start);
    }

    public async Task<ChatContext?> GetMostRecentChatContext(Guid userId)
    {
        return await db.ChatContexts.Where(x => x.UserId == userId)
                                        .OrderByDescending(x => x.CreatedAt)
                                        .FirstOrDefaultAsync();
    }

    public async Task<ChatEvent> AddChatEvent(ChatEvent chatEvent)
    {
        var result = await db.ChatEvents.AddAsync(chatEvent);

        await db.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<List<ChatEvent>> GetContextChatEvents(Guid contextId)
    {
        return await db.ChatEvents
            .Where(ce => ce.ChatContextId == contextId)
            .OrderByDescending(ce => ce.CreatedAt)
            .ToListAsync();
    }

    public async Task<User?> GetUserById(Guid authUserId)
    {
        return await db.Users.FirstOrDefaultAsync(x => x.AuthUserId == authUserId);
    }

    public async Task<UserProfile?> GetUserProfileById(Guid userId)
    {
        return await db.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId);
    }
}
