using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Linq;
using System.Threading.Tasks;

namespace sctm.services.discordBot.Commands.Reactions
{
    public partial class ReactionCommands
    {
        private async Task RunCommand_Error(DiscordClient discord, MessageReactionAddEventArgs e, DiscordDmChannel supportChannel)
        {
            var _react = e.Emoji;

            // reaction to embed
            if (e.Message.Embeds != null && e.Message.Embeds.Any())
            {
                var _embed = e.Message.Embeds[0];
                var _splitDescription = _embed.Footer.Text.Split(">>");
                //if (_splitDescription.Length != 2) return;

                var _area = _splitDescription[1].Split(':')[0].Trim();
                var _recordId = _splitDescription[1].Split(':')[1].Trim();

                //await e.Channel.SendMessageAsync("I see your thumbsdown");
                await e.Channel.SendMessageAsync($"I've marked your {_area} entry with Id: {_recordId} for follow-up. Sorry for the inconvenience");

                await supportChannel.SendMessageAsync("This was flagged as incorrect", false, _embed);
            }


        }
    }
}
