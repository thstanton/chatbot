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
        options.UseNpgsql(Environment.GetEnvironmentVariable("SUPABASE_CONNECTION_STRING"));
    });
});

var app = builder.Build();

await app.RunAsync();
