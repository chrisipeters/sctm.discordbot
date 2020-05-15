using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using sctm.connectors.azureComputerVision;
using sctm.logging;
using System.Net.Http;

namespace sctm.discordbot
{
    public partial class Commands
    {
        private IConfiguration _config;
        private FileLogger _logger;
        private DiscordClient _discordClient;
        private string _commandPreface;
        private AzureComputerVisionRepository _uploader;
        private HttpClient _httpClient;
        private DiscordEmbedBuilder _embed;

        public Commands(string commandPreface, IConfiguration config, FileLogger logger, DiscordClient client)
        {
            _config = config;
            _logger = logger;
            _discordClient = client;
            _commandPreface = commandPreface;

            _uploader = new AzureComputerVisionRepository(new AzureConfiguration
            {
                SubscriptionKey = _config["Azure:SubscriptionKey"],
                Endpoint = _config["Azure:Endpoint"]
            });

            _httpClient = new HttpClient();
            _embed = new DiscordEmbedBuilder();

        }
    }
}
