namespace ChatBot.Extensions;

public static class ChatEventExtensions
{
    public static string ToPromptString(this IEnumerable<ChatEvent> chatEvents, bool includeTimestamp = true)
    {
        var sb = new StringBuilder();

        foreach (var chat in chatEvents)
        {
            if (includeTimestamp)
            {
                sb.Append($"[{chat.Sender.ToString().ToUpper()} | {chat.CreatedAt:HH:mm}] ");
            }
            else
            {
                sb.Append($"[{chat.Sender.ToString().ToUpper()}] ");
            }

            sb.AppendLine(chat.Content);
        }

        return sb.ToString();
    }
}