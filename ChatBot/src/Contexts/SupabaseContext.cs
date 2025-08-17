namespace ChatBot.Contexts;

public class SupabaseContext(DbContextOptions<SupabaseContext> options) : DbContext(options)
{
    public DbSet<ChatContext> ChatContexts { get; set; }
    public DbSet<ChatEvent> ChatEvents { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<AgentId>();
        modelBuilder.HasPostgresEnum<Sender>();
    }
}