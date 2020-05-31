using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace sctm.services.discordBot.Commands.Attachments
{
    public partial class AttachmentCommands
    {
        private IConfiguration _config;
        private ILogger<Worker> _logger;
        private Services _services;
        private HttpClient _sctmClient;
        private HttpClient _openClient;
        private string _token;
        private DateTime _tokenDate;

        public AttachmentCommands(IConfiguration config, ILogger<Worker> logger, Services services)
        {
            _config = config;
            _logger = logger;
            _services = services;
            _sctmClient = new HttpClient();
            _openClient = new HttpClient();

            _tokenDate = DateTime.MinValue;
        }
    }
}
