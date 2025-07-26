using OpenAI.Chat;

namespace ChatBot.Services;

public class OpenAiService
{
    private static readonly string ApiKeyCredential = Environment.GetEnvironmentVariable("OPENAI_KEY")!;

    private readonly ChatClient _client = new(model: "gpt-4o-mini", apiKey: ApiKeyCredential);

    public async Task<ChatCompletion> TestCompletionAsync()
    {
        ChatCompletion response = await _client.CompleteChatAsync("Say 'This is a test'");

        Console.WriteLine($"[ASSISTANT] {response.Content[0].Text}");

        return response;
    }

    public async Task RespondToMessageAsync(string message)
    {
        var response = _client.CompleteChatStreamingAsync(message);

        Console.Write($"[ASSISTANT]: ");

        await foreach (var responseMessage in response)
        {
            if (responseMessage.ContentUpdate.Count > 0)
            {
                Console.Write(responseMessage.ContentUpdate[0].Text);
            }
        }

        Console.WriteLine();
    }

}

