using ChatBot.Contexts;

namespace ChatBot.Repositories;

public class SupabaseRepository(SupabaseContext db) : ISupabaseRepository
{
    public async Task<ChatContext> CreateChatContext(Guid userId, string systemPrompt)
    {
        var result = await db.ChatContexts.AddAsync(new ChatContext
        {
            UserId = userId,
            SystemPrompt = systemPrompt,
        });
        
        await db.SaveChangesAsync();
        
        return result.Entity;
    }

    public async Task<ChatContext?> GetChatContextByDate(Guid userId, DateTime date)
    {
        var start = date.Date.ToUniversalTime();
        var end = start.AddDays(1).ToUniversalTime();
        
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
