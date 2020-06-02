using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace sctm.services.discordBot.Commands.Messages
{
    public partial class MessageCommands
    {
        [Command("leaders")] // let's define this method as a command
        [Description("Request a DM from ChrispyKoala - this is ideal for commands you don't want to make in public")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("leaderboards")] // alternative names for the command
        public async Task AddCommands_Leaders(CommandContext ctx, [Description("desired leaderboard")]params string[] args)
        {
            
        }
    }
}
