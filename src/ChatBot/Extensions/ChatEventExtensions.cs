namespace ChatBot.Extensions;

public static class ChatEventExtensions
{
    public static string ToPromptString(this IEnumerable<ChatEvent> chatEvents, bool includeTimestamp = true)
    {
        var sb = new StringBuilder();

        foreach (var chat in chatEvents)
        {
            if (includeTimestamp)
                sb.Append($"[{chat.Sender.ToString().ToUpper()} | {chat.CreatedAt:HH:mm}] ");
            else
                sb.Append($"[{chat.Sender.ToString().ToUpper()}] ");

            sb.AppendLine(chat.Content);
        }

        return sb.ToString();
    }

    public static List<ChatMessage> ToChatMessages(this IEnumerable<ChatEvent> chatEvents)
    {
        var messages = new List<ChatMessage>();

        foreach (var chatEvent in chatEvents)
            switch (chatEvent.Sender)
            {
                case Sender.User:
                    messages.Add(new UserChatMessage(chatEvent.Content));
                    break;
                case Sender.Assistant:
                    messages.Add(new AssistantChatMessage(chatEvent.Content));
                    break;
                case Sender.System:
                    messages.Add(new SystemChatMessage(chatEvent.Content));
                    break;
                case Sender.Tool:
                    messages.Add(new ToolChatMessage(chatEvent.Content));
                    break;
                default:
                    continue;
            }

        return messages;
    }
}