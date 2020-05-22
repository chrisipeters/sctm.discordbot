using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace sctm.services.discordBot.Commands.Messages
{
    public partial class MessageCommands
    {
        private IConfiguration _config;
        private ILogger<Worker> _logger;
        private Services _services;
        private HttpClient _sctmClient;
        string _token = null;
        DateTime _tokenDate = DateTime.MinValue;

        public MessageCommands(IConfiguration config, ILogger<Worker> logger, Services services)
        {
            _config = config;
            _logger = logger;
            _services = services;
            _sctmClient = new HttpClient();
        }
    }
}
