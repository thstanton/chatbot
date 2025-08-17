using ChatBot.Contexts;

var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices(services =>
{
    services.AddHostedService<ConsoleService>();
    services.AddScoped<ISupabaseRepository, SupabaseRepository>();
    services.AddScoped<OrchestratorService>();
    services.AddSingleton<OpenAiService>();
    services.AddDbContext<SupabaseContext>(options =>
    {
        var connectionString = Environment.GetEnvironmentVariable("SUPABASE_CONNECTION_STRING");
        options
            .UseNpgsql(
                connectionString, 
                o => o
                    .MapEnum<AgentId>("agent_id")
                    .MapEnum<Sender>("sender_enum")
                    .MapEnum<Sex>("sex_enum")
                )
            .UseSnakeCaseNamingConvention();
    });
});

var app = builder.Build();

await app.RunAsync();
