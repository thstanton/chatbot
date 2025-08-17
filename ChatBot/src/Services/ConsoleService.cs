namespace ChatBot.Services;

public class ConsoleService(OrchestratorService orchestratorService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Welcome Human!");

        await orchestratorService.InitialiseChatSession(new Guid("bcdc81cc-98ae-44df-826f-284b53b19e93"));

        while (!stoppingToken.IsCancellationRequested)
        {
            Console.Write("> ");
            var userInput = Console.ReadLine();

            if (string.IsNullOrEmpty(userInput)) continue;

            if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
            
            await orchestratorService.HandleUserInput(userInput);

            Console.Write("[ASSISTANT]: ");

            await foreach (var chunk in orchestratorService.HandleAssistantResponse().WithCancellation(stoppingToken))
            {
                Console.Write(chunk);
            }
            
            Console.WriteLine();
        }
    }
}