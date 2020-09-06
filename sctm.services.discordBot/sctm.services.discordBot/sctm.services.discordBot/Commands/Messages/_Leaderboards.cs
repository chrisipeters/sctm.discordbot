using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace sctm.services.discordBot.Commands.Messages
{
    public partial class MessageCommands
    {

        [Command("leaderboards")] // let's define this method as a command
        [Description("Get leaderboard - Mining")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("leaders")] // alternative names for the command
        public async Task AddCommands_Leaderboards(CommandContext ctx, [Description("Leaderboard")] params string[] args)
        {
            var _leaderboard = string.Join(' ', args);
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":mag:"));

            if (_leaderboard.Trim().ToLower() == "mining")
            {
                var _data = await _services.GetLeaderboards_Organization(ctx.Guild.Id.ToString());
                if(_data != null)
                {
                    var _embed = await Embeds.Leaderboard_Mining(ctx, ctx.Client.CurrentUser, _data);

                    await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromName(ctx.Client, ":mag:"));

                    await ctx.RespondAsync(null, false, _embed);
                }
            }
        }        
    }
}
