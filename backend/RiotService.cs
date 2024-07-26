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
        public string? TagLine { get; set; }
        public string? Puuid { get; set; }
        public string? GameName { get; set; }
        public string? AccountId { get; set; }
        public string? SummonerId { get; set; }
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
        }
        public void UpdateSettings(RiotSettings settings)
        {
            _settings = settings;
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
            Console.WriteLine("Sending request to :"+url);
            var response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Request failed with status code {response.Content}");
            }
            return JsonNode.Parse(response.Content);
        }
        public JsonNode GetSummonerId()
        {
            if(string.IsNullOrEmpty(_settings.Puuid)&&string.IsNullOrEmpty(_settings.RegionTag))
            {
                throw new InvalidOperationException("RegionTag and Puuid must be set.");
            }
            var queries= new Dictionary<string,string>
            {
                {"api_key", apiKey}
            };
            var accountInfos = GetData($"https://{_settings.RegionTag}.api.riotgames.com/lol/summoner/v4/summoners/by-puuid/{_settings.Puuid}",queries);
            if (accountInfos == null)
            {
                throw new ArgumentNullException(nameof(accountInfos));
            }
            string summonerId = accountInfos.AsObject()["id"].ToString();
            string accoundId = accountInfos.AsObject()["accountId"].ToString();
            if (!string.IsNullOrEmpty(summonerId) && !string.IsNullOrEmpty(accoundId))
            {
                _settings.SummonerId = summonerId;
                _settings.AccountId = accoundId;
            }
            else
            {
                throw new InvalidOperationException("Summoner Id or Account Id not found in the response.");
            }
            return accountInfos;
        }
        public backend.RiotSettings GetSummonerInfo()
        {
            if (string.IsNullOrEmpty(_settings.SummonerName) || string.IsNullOrEmpty(_settings.Region) || string.IsNullOrEmpty(_settings.RegionTag))
            {
                throw new InvalidOperationException("Summoner name and region tag must be set.");
            }
            var queries= new Dictionary<string,string>
            {
                {"api_key", apiKey}
            };
            var tag = "";
            if(_settings.TagLine!=null)
            {
                tag = _settings.TagLine;
            }
            else 
            {
                tag = _settings.RegionTag;
            }
            var summoner = GetData($"https://{_settings.Region}.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{_settings.SummonerName}/{tag}", queries);
            if (summoner == null)
            {
                throw new ArgumentNullException(nameof(summoner));
            }
            var puuid = summoner.AsObject()["puuid"].ToString();
            var GameName = summoner.AsObject()["gameName"].ToString();
            if (!string.IsNullOrEmpty(puuid) && !string.IsNullOrEmpty(GameName))
            {
                _settings.Puuid = puuid;
                _settings.GameName = GameName;
                GetSummonerId();
            }
            else
            {
                throw new InvalidOperationException("Puuid or Game Name not found in the response.");
            }
            return _settings;
        }
        public JsonNode GetMatchHistoryGameIds([FromQuery] string? Region, [FromQuery] string? Puuid, [FromQuery] int? idStartList, [FromQuery] int? idEndList)
        {
            if(string.IsNullOrEmpty(_settings.Region) || string.IsNullOrEmpty(_settings.Puuid))
            {
                throw new InvalidOperationException("Region and the puuid must be set.");
            }
            var queries= new Dictionary<string,string>
            {
                {"api_key", apiKey}
            };
            if(idStartList!=null && idStartList>0 )
            {
                if(idEndList!=null && idStartList<=idEndList)
                {
                    queries.Add("start",idStartList.ToString());
                    queries.Add("count",idEndList.ToString());
                }
                else if(idEndList==null)
                {
                    queries.Add("start",idStartList.ToString());
                }
            }
            else if(idEndList!=null && idEndList>0)
            {
                queries.Add("count",idEndList.ToString());
            }
            var matchIds = GetData($"https://{_settings.Region}.api.riotgames.com/lol/match/v5/matches/by-puuid/{_settings.Puuid}/ids",queries);
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
            var matchData = GetData($"https://{_settings.Region}.api.riotgames.com/lol/match/v5/matches/{matchId}",queries);
            if (matchData == null)
            {
                throw new ArgumentNullException(nameof(matchData));
            }
            return matchData;
        }
    }
}