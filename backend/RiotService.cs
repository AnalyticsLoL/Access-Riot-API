using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace backend
{
    public class RiotSettings
    {
        public string? SummonerName { get; set; }
        public string? Region { get; set; }
        public string? RegionTag { get; set; }
        public string? Puuid { get; set; }
        public string? GameName { get; set; }
    }
    public class RiotService
    {
        private readonly HttpClient _httpClient;
        public RiotSettings _settings;
        private static readonly string apiKey = new ApiKey().key;
        private string _baseUrl;
        public RiotService(HttpClient httpClient, RiotSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            UpdateBaseUrl();
        }
        private void UpdateBaseUrl()
        {
            _baseUrl = $"https://{_settings.Region}.api.riotgames.com";
        }
        public void UpdateSettings(RiotSettings settings)
        {
            _settings = settings;
            UpdateBaseUrl();
        }
        private static JsonNode GetData(string url, Dictionary<string,string> queries)
        {
            var options = new RestClientOptions(url);
            var client = new RestClient(options);
            var request = new RestRequest("");
            foreach (var query in queries)
            {
                request.AddParameter(query.Key, query.Value);
            }
            var response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Request failed with status code {response.Content}");
            }
            return JsonNode.Parse(response.Content);
        }
        public JsonNode GetSummonerInfo()
        {
            if (string.IsNullOrEmpty(_settings.SummonerName) || string.IsNullOrEmpty(_settings.Region) || string.IsNullOrEmpty(_settings.RegionTag))
            {
                throw new InvalidOperationException("Summoner name and region tag must be set.");
            }
            var queries= new Dictionary<string,string>
            {
                {"api_key", apiKey}
            };
            var summoner = GetData($"{_baseUrl}/riot/account/v1/accounts/by-riot-id/{_settings.SummonerName}/{_settings.RegionTag}", queries);

            if (summoner == null)
            {
                throw new ArgumentNullException(nameof(summoner));
            }
            var puuid = summoner.AsObject()["puuid"];
            var GameName = summoner.AsObject()["gameName"];
            if (puuid != null && GameName != null)
            {
                _settings.Puuid = puuid.ToString();
                _settings.GameName = GameName.ToString();
            }
            else
            {
                throw new InvalidOperationException("Puuid or Game Name not found in the response.");
            }
            return summoner;
        }

        public JsonNode GetMatchHistoryGameIds()
        {
            if(string.IsNullOrEmpty(_settings.Region) || string.IsNullOrEmpty(_settings.Puuid))
            {
                throw new InvalidOperationException("Region and the puuid must be set.");
            }
            var queries= new Dictionary<string,string>
            {
                {"api_key", apiKey}
            };
            var matchIds = GetData($"{_baseUrl}/lol/match/v5/matches/by-puuid/{_settings.Puuid}/ids",queries);
            if (matchIds == null)
            {
                throw new ArgumentNullException(nameof(matchIds));
            }
            return matchIds;
        }

        public JsonNode GetMatchInfos([FromQuery] string matchId)
        {
            if(string.IsNullOrEmpty(_settings.Region))
            {
                throw new InvalidOperationException("Region must be set.");
            }
            var queries= new Dictionary<string,string>
            {
                {"api_key", apiKey}
            };
            var matchData = GetData($"{_baseUrl}/lol/match/v5/matches/{matchId}",queries);
            if (matchData == null)
            {
                throw new ArgumentNullException(nameof(matchData));
            }
            return matchData;
        }
    }
}

