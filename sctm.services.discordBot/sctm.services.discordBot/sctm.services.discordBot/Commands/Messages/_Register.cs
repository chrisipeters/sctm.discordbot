using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
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
            var _dmChannel = await ctx.CommandsNext.Client.CreateDmAsync(ctx.Message.Author);

            string serverName = (ctx.Message.Channel.Guild?.Name != null) ? ctx.Message.Channel.Guild.Name : "SCTradeMasters";
            string serverAvatarUrl = (ctx.Message.Channel.Guild?.IconUrl != null) ? ctx.Message.Channel.Guild.IconUrl : ctx.Client.CurrentUser.AvatarUrl;
            ulong serverId = (ctx.Message.Channel.Guild?.Id != null) ? ctx.Message.Channel.Guild.Id : ctx.Client.CurrentUser.Id;
            string botAvatarUrl = ctx.Client.CurrentUser.AvatarUrl;

            var _embed = Embeds.Register(serverName, serverAvatarUrl, serverId, botAvatarUrl);
            await _dmChannel.SendMessageAsync(null, false, _embed);
        }
    }
}
