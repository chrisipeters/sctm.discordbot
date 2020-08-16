using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;

namespace sctm.services.discordBot
{
    public partial class Services
    {
        private IConfiguration _config;
        private DiscordConfiguration _cfg;
        private DiscordClient _discord;
        private CommandsNextConfiguration _ccfg;
        private CommandsNextModule _commands;
        private string _token;
        private DateTime _tokenDate;

        public Services(IConfiguration config)
        {
            _config = config;
            _cfg = new DiscordConfiguration
            {
                Token = config["Discord:Token"],
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LogLevel = DSharpPlus.LogLevel.Debug,
                UseInternalLogHandler = false
            };
        }
    }
}
