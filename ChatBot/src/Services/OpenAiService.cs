namespace ChatBot.Services;

public class OpenAiService() : IOpenAiService
{
    private static readonly string ApiKeyCredential = Environment.GetEnvironmentVariable("OPENAI_KEY")!;

    private readonly ChatClient _client = new(model: "gpt-4o-mini", apiKey: ApiKeyCredential);

    public async Task<ChatCompletion> TestCompletionAsync()
    {
        ChatCompletion response = await _client.CompleteChatAsync("Say 'This is a test'");

        Console.WriteLine($"[ASSISTANT] {response.Content[0].Text}");

        return response;
    }

    public async Task<string> GetCompletionAsync(string prompt)
    {
        ChatCompletion response = await _client.CompleteChatAsync(prompt);

        return response == null ? string.Empty : response.Content[0].Text;
    }

    public async IAsyncEnumerable<string> RespondToMessageAsync(ChatMessage[] messages)
    {
        var response = _client.CompleteChatStreamingAsync(messages);

        await foreach (var responseMessage in response)
        {
            if (responseMessage.ContentUpdate.Count > 0)
            {
                yield return responseMessage.ContentUpdate[0].Text;
            }
        }
    }

}

