using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace sctm.services.discordBot
{
    public partial class Processors
    {
        private IConfiguration _config;
        private ILogger<Worker> _logger;
        private Services _services;
        private DiscordDmChannel _supportChannel;
        private CommandContext _ctx;

        public Processors(IConfiguration config, ILogger<Worker> logger, Services services, DiscordDmChannel supportChannel)
        {
            _config = config;
            _logger = logger;
            _services = services;
            _supportChannel = supportChannel;
        }
    }
}
