using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace sctm.services.discordBot.Commands.Reactions
{
    public partial class ReactionCommands
    {
        private IConfiguration _config;
        private ILogger<Worker> _logger;
        private Services _services;
        private CommandContext _ctx;

        public ReactionCommands(IConfiguration config, ILogger<Worker> logger, Services services)
        {
            _config = config;
            _logger = logger;
            _services = services;
        }

        public async Task RunCommand_Reaction(DiscordClient discord, MessageReactionAddEventArgs e, DiscordDmChannel supportChannel)
        {
            var _react = e.Emoji;
            var _ocrReaction = new List<DiscordEmoji> { DiscordEmoji.FromName(discord, ":mag:"), DiscordEmoji.FromName(discord, ":mag_right:") };
            var _galleryReaction = new List<DiscordEmoji> { DiscordEmoji.FromName(discord, ":camera:") };
            var _errorReaction = new List<DiscordEmoji> { DiscordEmoji.FromName(discord, ":thumbsdown:") };

            if (_ocrReaction.Contains(_react))
            {
                // ocr processor
                await RunCommand_OCR(discord, e, supportChannel);
            }
            else if (_galleryReaction.Contains(_react))
            {
                // gallery processor
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discord, ":camera_with_flash:"));
                await e.Message.RespondAsync("Gallery submissions are coming soon!");
            }
            else if (_errorReaction.Contains(_react))
            {
                // error reaction
                await RunCommand_Error(discord, e, supportChannel);
            }
            else return;

        }
    }
}
