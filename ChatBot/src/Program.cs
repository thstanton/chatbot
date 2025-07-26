using ChatBot.Repositories;
using ChatBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices(services =>
{
    services.AddHostedService<OrchestratorService>();
    services.AddSingleton<OpenAiService>();
    services.AddHttpClient<SupabaseRepository>();
});

var app = builder.Build();

await app.RunAsync();
