using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace sctm.services.discordBot.Commands.Messages
{
    public partial class MessageCommands
    {
        [Command("register")] // let's define this method as a command
        [Description("Register to join SCTradeMasters Services")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("join")] // alternative names for the command
        public async Task AddCommands_Register(CommandContext ctx)
        {
            var _dmChannel = await ctx.CommandsNext.Client.CreateDmAsync(ctx.User);
            var _embed = Embeds.Register(ctx.Message, ctx.Client.CurrentUser.AvatarUrl);
            await _dmChannel.SendMessageAsync(null, false, _embed);
        }
    }
}
