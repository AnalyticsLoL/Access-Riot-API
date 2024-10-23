using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RiotDataController : ControllerBase
    {
        private readonly RiotService _riotService;

        public RiotDataController(RiotService riotService)
        {
            _riotService = riotService;
        }
        [HttpPost("settings")]
        public IActionResult UpdateSettings([FromBody] RiotSettings settings)
        {
            if (settings == null)
            {
                return BadRequest("Settings cannot be null.");
            }

            // Update RiotService with new settings
            _riotService.UpdateSettings(settings);
            _riotService.GetSummonerInfo();

            return Ok(_riotService._settings);
        }
        [HttpGet("summoner")]
        public IActionResult GetSummonerInfo()
        {
            var summonerData = _riotService.GetSummonerInfo();
            return Ok(summonerData);
        }
        [HttpGet("summonerId")]
        public IActionResult GetSummonerId()
        {
            var summonerId = _riotService.GetSummonerId();
            return Ok(summonerId);
        }
        [HttpGet("matchhistory")]
        public IActionResult GetMatchHistory([FromQuery] string? region, [FromQuery] string? puuid, [FromQuery] int? idStartList, [FromQuery] int? idEndList)
        {
            var matchIds = _riotService.GetMatchHistoryGameIds(region, puuid, idStartList, idEndList);
            return Ok(matchIds);
        } 
        [HttpGet("matchinfo")]
        public IActionResult GetMatchInfos([FromQuery] string matchId)
        {
            var matchInfo = _riotService.GetMatchInfos(matchId);
            return Ok(matchInfo);
        }       
    }
}