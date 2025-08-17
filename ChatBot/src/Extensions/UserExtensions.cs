namespace ChatBot.Extensions;

public static class UserExtensions
{
    public static int GetAge(this DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;

        // If birthday hasn't occurred yet this year, subtract 1
        if (dateOfBirth.Date > today.AddYears(-age)) 
            age--;

        return age;
    }
}