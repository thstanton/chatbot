using ChatBot.Contexts;

var builder = Host.CreateDefaultBuilder(args);

Console.WriteLine($"env: {Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}");

builder.ConfigureServices(services =>
{
    services.AddSingleton<IOpenAiService, OpenAiService>();
    services.AddTransient<IOrchestratorService, OrchestratorService>();
    services.AddScoped<ISupabaseRepository, SupabaseRepository>();
    services.AddScoped<ChatSessionService>();
    services.AddHostedService<ConsoleService>();

    services.AddDbContextPool<SupabaseContext>(options =>
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
