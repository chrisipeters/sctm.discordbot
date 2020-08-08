using Microsoft.Extensions.Configuration;

namespace sctm.services.discordBot.Commands.Messages
{
    public partial class MessageCommands
    {
        private IConfiguration _config;
        private Services _services;

        public MessageCommands(IConfiguration config, Services services)
        {
            _config = config;
            _services = services;
        }
    }
}
