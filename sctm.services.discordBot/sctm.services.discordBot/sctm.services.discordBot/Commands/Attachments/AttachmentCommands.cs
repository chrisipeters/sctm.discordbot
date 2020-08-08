using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;

namespace sctm.services.discordBot.Commands.Attachments
{
    public partial class AttachmentCommands
    {
        private IConfiguration _config;
        private Services _services;
        private CommandContext _ctx;

        public AttachmentCommands(IConfiguration config, Services services)
        {
            _config = config;
            _services = services;
        }
    }
}
