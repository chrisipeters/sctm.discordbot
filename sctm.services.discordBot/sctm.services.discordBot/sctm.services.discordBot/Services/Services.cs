using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Net.Http;

namespace sctm.services.discordBot
{
    public partial class Services
    {
        private IConfiguration _config;
        private DiscordConfiguration _cfg;
        private ILogger<Worker> _logger;
        private HttpClient _httpClient;
        private DiscordClient _discord;
        private CommandsNextConfiguration _ccfg;
        private CommandsNextModule _commands;
        private string _token;
        private DateTime _tokenDate;

        public Services(IConfiguration config, ILogger<Worker> logger)
        {
            _config = config;
            _cfg = new DiscordConfiguration
            {
                Token = config["Discord:DevToken"],
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LogLevel = DSharpPlus.LogLevel.Debug,
                UseInternalLogHandler = false
            };

            _logger = logger;
            _httpClient = new HttpClient();
        }
    }
}
