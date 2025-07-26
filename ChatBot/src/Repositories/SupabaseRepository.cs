using ChatBot.Services;
using Microsoft.Extensions.Configuration;

namespace ChatBot.Repositories;

public class SupabaseRepository(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;
    
    
}
