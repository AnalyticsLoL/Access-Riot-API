using System;
using System.Net.Http;
using System.Threading.Tasks;
using AccessRiotAPI;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {   
        ApiKey apiKey = new ApiKey();
        string summonerName = "Sarenberi#NA1";
        string region = "na1";

        string requestUrl = $"https://{region}.api.riotgames.com/lol/summoner/v4/summoners/{summonerName}?api_key={apiKey.key}";
        Console.WriteLine($"Sending request to: {requestUrl}");

        try
        {
            HttpResponseMessage response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
        }
    }
}