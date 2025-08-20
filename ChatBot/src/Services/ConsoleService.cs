namespace ChatBot.Services;

public class ConsoleService(IServiceProvider serviceProvider, ILogger<ConsoleService> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Welcome Human!");
        
        using var scope = _serviceProvider.CreateScope();

        var chatSessionService = scope.ServiceProvider.GetService<ChatSessionService>();

        if (chatSessionService == null)
        {
            logger.LogWarning("Could not find orchestrator service.");
            return;
        }

        await chatSessionService.InitialiseChatSession(new Guid("bcdc81cc-98ae-44df-826f-284b53b19e93"));

        while (!stoppingToken.IsCancellationRequested)
        {
            Console.Write("> ");
            var userInput = Console.ReadLine();

            if (string.IsNullOrEmpty(userInput)) continue;

            if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
            
            await chatSessionService.HandleUserInput(userInput);

            Console.Write("[ASSISTANT]: ");

            await foreach (var chunk in chatSessionService.HandleAssistantResponse().WithCancellation(stoppingToken))
            {
                Console.Write(chunk);
            }
            
            Console.WriteLine();
        }
    }
}