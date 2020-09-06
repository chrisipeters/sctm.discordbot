using Newtonsoft.Json;
using sctm.services.discordBot.Models;
using Serilog;
using System.Threading.Tasks;

namespace sctm.services.discordBot
{
    public partial class Services
    {
        public async Task<Leaderboards_Result> GetLeaderboards_Global()
        {
            var _url = _config["SCTM:Urls:Leaderboards"].TrimEnd('/') + $"/global";
            var _data = await GetLeaderboards(_url);
            return _data;
        }

        public async Task<Leaderboards_Result> GetLeaderboards_Organization(string organizationIdentifier)
        {
            var _url = _config["SCTM:Urls:Leaderboards"].TrimEnd('/') + $"/organizations/{organizationIdentifier}";
            var _data = await GetLeaderboards(_url);
            return _data;
        }

        public async Task<Leaderboards_Result> GetLeaderboards_Player(string userIdentifier)
        {
            var _url = _config["SCTM:Urls:Leaderboards"].TrimEnd('/') + $"/players/{userIdentifier}";
            var _data = await GetLeaderboards(_url);
            return _data;
        }


        private async Task<Leaderboards_Result> GetLeaderboards(string url)
        {
            var _logAction = "GetLeaderboards";
            Log.Information($"{_logAction} -  command called");

            Leaderboards_Result _ret = null;

            var _client = await GetSCTMClient();
            if (_client == null)
            {
                Log.Error("{logAction}: unable to get SCTM client", _logAction);
                return null;
            }

            var _res = await _client.GetAsync(url);

            var _content = await _res.Content.ReadAsStringAsync();

            if(!_res.IsSuccessStatusCode)
            {
                Log.Error("{logAction}: error received from {url} - {@content}",_logAction,url, _content);
                _ret = null;
            } else
            {
                _ret = JsonConvert.DeserializeObject<Leaderboards_Result>(_content);
            }

            _client.Dispose();
            return _ret;
        }
    }
}
