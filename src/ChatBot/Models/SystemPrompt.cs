namespace ChatBot.Models;

public class SystemPrompt
{
    public SystemPrompt(User user, UserProfile? userProfile, string basePrompt,
        string? chatSummary)
    {
        var prompt = basePrompt;

        if (userProfile != null)
        {
            var userPrompt =
                $"Your client is {user.DisplayName}, a {userProfile.DateOfBirth.GetAge()} year old {userProfile.Sex.ToString()}." +
                $"Their height is {userProfile.Height}{userProfile.HeightUnit}. " +
                $"Their current goal is {userProfile.Goal}.";

            prompt += userPrompt;
        }

        if (chatSummary != null) prompt += $"\nPrevious conversation summary: {chatSummary}";

        Message = prompt;
    }

    public string Message { get; set; }
}