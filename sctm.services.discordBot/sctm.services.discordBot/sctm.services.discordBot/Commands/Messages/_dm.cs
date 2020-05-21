using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace sctm.services.discordBot.Commands.Messages
{
    public partial class MessageCommands
    {
        [Command("dm")] // let's define this method as a command
        [Description("Request a DM from ChrispyKoala - this is ideal for commands you don't want to make in public")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("chat","pm")] // alternative names for the command
        public async Task AddCommands_DM(CommandContext ctx)
        {
            var _dmChannel = await ctx.CommandsNext.Client.CreateDmAsync(ctx.User);
            await _dmChannel.SendMessageAsync("Hi there! What can I do for you today?");
        }
    }
}
