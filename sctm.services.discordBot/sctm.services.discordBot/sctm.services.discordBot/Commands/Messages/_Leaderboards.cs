using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Serilog;
using System;
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
            var _logAction = "AddCommands_Leaderboards";
            var _leaderboard = string.Join(' ', args);
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":mag:"));

            if (_leaderboard.Trim().ToLower() == "mining")
            {
                try
                {
                    var _data = await _services.GetLeaderboards_Organization(ctx.Guild.Id.ToString());
                    if (_data != null)
                    {
                        var _embed = await Embeds.Leaderboard_Mining(ctx, ctx.Client.CurrentUser, _data);



                        await ctx.RespondAsync(null, false, _embed);
                    }
                    else
                    {
                        await ctx.Message.RespondAsync("There is no active season running - Previous season results will be available on our website soon");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "{logAction}: Error getting mining leaderboard data: {error}", _logAction, ex.Message);

                    //await supportChannel.SendMessageAsync($"Error getting mining leaderboards: {ex.Message}");

                    await ctx.Message.RespondAsync("I wasn't able to retrieve any active season data...");
                }
                

                await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromName(ctx.Client, ":mag:"));
            }
        }        
    }
}
