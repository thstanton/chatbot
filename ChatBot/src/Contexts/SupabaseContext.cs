namespace ChatBot.Contexts;

public class SupabaseContext(DbContextOptions<SupabaseContext> options) : DbContext(options)
{
    public DbSet<ChatContext> ChatContexts { get; set; }
    public DbSet<ChatEvent> ChatEvents { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
}