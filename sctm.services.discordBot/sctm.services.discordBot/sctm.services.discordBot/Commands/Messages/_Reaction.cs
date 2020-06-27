using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace sctm.services.discordBot.Commands.Messages
{
    public partial class MessageCommands
    {
        public async static Task RunCommand_Reaction(DiscordClient discord, MessageReactionAddEventArgs e, DiscordDmChannel supportChannel)
        {
            var _react = e.Emoji;
            var _reactName = _react.Name.ToLower();
            var _reactDiscordName = _react.GetDiscordName();
            var _terminal = DiscordEmoji.FromName(discord, ":desktop:");

            // submit as gallery
            if (_react == DiscordEmoji.FromName(discord, ":camera:"))
            {
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discord, ":camera_with_flash:"));
                await e.Message.RespondAsync("Gallery submissions are coming soon!");
            }

            // submit as trade terminal
            else if (_react == DiscordEmoji.FromName(discord, ":computer:"))
            {
                await e.Message.RespondAsync("Processing image as a data screenshot");

            }

            else if (e.Message.Author != null && e.Message.Author.IsCurrent)
            {
                await RunCommand_ReactionToBot(discord, e, supportChannel);
            }
            else
            {
                //await e.Channel.SendMessageAsync($"I see your {_react} to someone else's message: {e.Message.Content?.Substring(0, 10)} ");
            }


        }

        private static async Task RunCommand_ReactionToBot(DiscordClient discord, MessageReactionAddEventArgs e, DiscordDmChannel supportChannel)
        {
            var _react = e.Emoji;

            // reaction to embed
            if (e.Message.Embeds != null && e.Message.Embeds.Any())
            {
                var _embed = e.Message.Embeds[0];
                var _splitDescription = _embed.Footer.Text.Split(">>");
                if (_splitDescription.Length != 2) return;

                var _area = _splitDescription[1].Split(':')[0].Trim();
                var _recordId = _splitDescription[1].Split(':')[1].Trim();

                if (_react == DiscordEmoji.FromName(discord, ":thumbsdown:"))
                {
                    //await e.Channel.SendMessageAsync("I see your thumbsdown");
                    await e.Channel.SendMessageAsync($"I've marked your {_area} entry with Id: {_recordId} for follow-up. Sorry for the inconvenience");

                    await supportChannel.SendMessageAsync("This was flagged as incorrect", false, _embed);
                }
            }


        }
    }
}
