using Microsoft.Extensions.Hosting;

namespace ChatBot.Services;

public class OrchestratorService(OpenAiService openAiService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Welcome Human!");

        await openAiService.TestCompletionAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            Console.Write("> ");
            var userInput = Console.ReadLine();

            if (string.IsNullOrEmpty(userInput)) continue;

            if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

            await openAiService.RespondToMessageAsync(userInput);
        }
    }
}