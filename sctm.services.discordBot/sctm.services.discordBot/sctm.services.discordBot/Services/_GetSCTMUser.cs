using Newtonsoft.Json;
using sctm.services.discordBot.Models;
using Serilog;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace sctm.services.discordBot
{
    public partial class Services
    {
        public async Task<SCTMUser> GetUser(ulong discordId)
        {
            SCTMUser _ret = null;
            if (_token == null || _tokenDate < DateTime.Now.AddMinutes(-15))
            {
                _token = await _GetSCTMToken();
                if (_token != null) _tokenDate = DateTime.Now;
            }

            if (_tokenDate != null && _tokenDate > DateTime.Now.AddMinutes(-15))
            {
                _sctmHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                var _url = _config["SCTM:Urls:GetMember"].TrimEnd('/') + "/" + discordId;

                var _result = await _sctmHttpClient.GetAsync(_url);
                if (!_result.IsSuccessStatusCode) return null;
                else
                {
                    try
                    {
                        var _json = await _result.Content.ReadAsStringAsync();
                        _ret = JsonConvert.DeserializeObject<SCTMUser>(_json);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error getting user from SCTM");
                        _ret = null;
                    }
                }
            }

            return _ret;
        }
    }
}
