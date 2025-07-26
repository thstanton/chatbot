using ChatBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateApplicationBuilder();

host.Services.AddHostedService<OrchestratorService>();
host.Services.AddSingleton<OpenAiService>();

host.Build().Run();
