namespace ChatBot.Services;

public class ConsoleService(OrchestratorService orchestratorService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Welcome Human!");

        await orchestratorService.InitialiseChatSession();

        while (!stoppingToken.IsCancellationRequested)
        {
            Console.Write("> ");
            var userInput = Console.ReadLine();

            if (string.IsNullOrEmpty(userInput)) continue;

            if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

            Console.Write("[ASSISTANT]: ");

            await foreach (var chunk in openAiService.RespondToMessageAsync(userInput).WithCancellation(stoppingToken))
            {
                Console.Write(chunk);
            }
            
            Console.WriteLine();
        }
    }
}