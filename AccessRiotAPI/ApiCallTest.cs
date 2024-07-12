using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RiotApiTest
{
    class Program
    {
        private static readonly string apiKey = "";
        private static readonly string summonerName = "darthmomo18";
        private static readonly string region = "americas";

        private static readonly string regionTag = "NA1";
        private static readonly string baseUrl = $"https://{region}.api.riotgames.com/riot";

        static async Task Main(string[] args)
        {
            try
            {
                var puuid = await GetPuuidBySummonerName(summonerName);
                //var matchHistory = await GetMatchHistoryByGameId(puuid);

                Console.WriteLine("Match History:");
                /*foreach (var match in matchHistory)
                {
                    Console.WriteLine(match);
                }*/
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task<string> GetPuuidBySummonerName(string summonerName)
        {
            using (HttpClient client = new HttpClient())
            {
                var url = $"https://americas.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{summonerName}/NA1?api_key={apiKey}";
                
                var response = await client.GetStringAsync(url);
                Console.WriteLine(response);
                var summoner = JsonContent.DeserializeObject<Summoner>(response);

                if (summoner == null)
                {
                    throw new ArgumentNullException(nameof(summoner));
                }

                return summoner.Puuid;
            }
        }

        private static async Task<List<string>> GetMatchHistoryByGameId(string gameID)
        {
            using (HttpClient client = new HttpClient())
            {
                var matchUrl = $"https://americas.api.riotgames.com/lol/match/v5/matches/{gameID}?api_key={apiKey}";
                //client.DefaultRequestHeaders.Add("X-Riot-Token", apiKey);
                var response = await client.GetStringAsync(matchUrl);
                var matchHistory = JsonConvert.DeserializeObject<List<string>>(response);
                return matchHistory;
            }
        }
    }

    class Summoner
    {
        [JsonProperty("tagLine")]
        public string Id { get; set; }

        [JsonProperty("gameName")]
        public string AccountId { get; set; }

        [JsonProperty("puuid")]
        public string Puuid { get; set; }

    }
}
