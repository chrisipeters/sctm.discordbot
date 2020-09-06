using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Net.WebSocket;
using Newtonsoft.Json;
using sctm.connectors.sctmDB.Models.Experience;
using Serilog;
using System.Threading.Tasks;

namespace sctm.services.discordBot
{
    public partial class Services
    {
        public async Task<ExperienceResultSummary> GetExperience_Global()
        {
            var _url = _config["SCTM:Urls:Experience"].TrimEnd('/') + $"/global";
            var _data = await GetExperience(_url);
            return _data;
        }

        public async Task<ExperienceResultSummary> GetExperience_Organization(string organizationIdentifier)
        {
            var _url = _config["SCTM:Urls:Experience"].TrimEnd('/') + $"/organizations/{organizationIdentifier}";
            var _data = await GetExperience(_url);
            return _data;
        }

        public async Task<ExperienceResultSummary> GetExperience_Player(string userIdentifier)
        {
            var _url = _config["SCTM:Urls:Experience"].TrimEnd('/') + $"/players/{userIdentifier}";
            var _data = await GetExperience(_url);
            return _data;
        }


        private async Task<ExperienceResultSummary> GetExperience(string url)
        {
            var _logAction = "GetExperience";
            Log.Information($"{_logAction} -  command called");

            ExperienceResultSummary _ret = null;

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
                _ret = JsonConvert.DeserializeObject<ExperienceResultSummary>(_content);
            }

            _client.Dispose();
            return _ret;
        }
    }
}
