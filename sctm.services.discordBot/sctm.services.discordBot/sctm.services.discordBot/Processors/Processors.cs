using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;

namespace sctm.services.discordBot
{
    public partial class Processors
    {
        private IConfiguration _config;
        private Services _services;
        private DiscordDmChannel _supportChannel;
        private CommandContext _ctx;

        public Processors(IConfiguration config, Services services, DiscordDmChannel supportChannel)
        {
            _config = config;
            _services = services;
            _supportChannel = supportChannel;
        }
    }
}
