using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace sctm.services.discordBot.Commands.Messages
{
    public partial class MessageCommands
    {
        private IConfiguration _config;
        private ILogger<Worker> _logger;
        private Services _services;

        public MessageCommands(IConfiguration config, ILogger<Worker> logger, Services services)
        {
            _config = config;
            _logger = logger;
            _services = services;
        }
    }
}
