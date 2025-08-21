namespace ChatBot.Contexts;

public class SupabaseContext(DbContextOptions<SupabaseContext> options) : DbContext(options)
{
    public DbSet<ChatContext> ChatContexts { get; set; }
    public DbSet<ChatEvent> ChatEvents { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatContext>().Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        modelBuilder.Entity<User>().Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        modelBuilder.Entity<UserProfile>().Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        modelBuilder.Entity<ChatEvent>().Property(e => e.CreatedAt).HasDefaultValueSql("now()");
    }
}