using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace sctm.discordbot
{
    public partial class Services
    {
        public async Task<string> GetToken()
        {
            var _logAction = "GetToken";

            var _loginUrl = _config["SCTM:Urls:Login"];
            var _email = _config["SCTM:Account:Email"]; ;
            var _password = _config["SCTM:Account:Password"];

            _logger.WriteEntry(new logging.Models.LogEntry
            {
                Action = _logAction,
                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                Message = $"Getting JWT Token from: {_loginUrl}"
            });


            if (_token == null || _tokenDate < (DateTime.Now.AddMinutes(-15)))
            {
                var _loginModel = new SCTMLoginModel
                {
                    Email = _email,
                    Password = _password,
                };

                var _json = JsonConvert.SerializeObject(_loginModel);

                HttpContent c = new StringContent(_json, Encoding.UTF8, "application/json");

                var _res = await _httpClient.PostAsync(_loginUrl, c);
                if (_res.IsSuccessStatusCode)
                {
                    _logger.WriteEntry(new logging.Models.LogEntry
                    {
                        Action = _logAction,
                        Level = Microsoft.Extensions.Logging.LogLevel.Information,
                        Message = $"Getting JWT Token - success"
                    });

                    var _result = JsonConvert.DeserializeObject<SCTMLoginResponse>(await _res.Content.ReadAsStringAsync());
                    _token = _result.JWT;
                    _tokenDate = DateTime.Now;
                }
                else
                {
                    _logger.WriteEntry(new logging.Models.LogEntry
                    {
                        Action = _logAction,
                        Level = Microsoft.Extensions.Logging.LogLevel.Error,
                        Message = $"Getting JWT Token - Error"
                    });
                    _token = null;
                    _tokenDate = DateTime.MinValue;
                }
            }

            return _token;
        }

        public class SCTMLoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class SCTMLoginResponse
        {
            public string JWT { get; set; }
            public string RefreshToken { get; set; }
            public string AccessToken { get; set; }
            public int Intel { get; set; }
            public int Prestige { get; set; }
            public string Experience { get; set; }
        }
    }
}
