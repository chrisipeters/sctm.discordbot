using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace sctm.discordbot.Commands
{
    public partial class MessageCommands
    {
        [Command("ping")] // let's define this method as a command
        [Description("Example ping command")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("pong")] // alternative names for the command
        public async Task AddCommands_Ping(CommandContext ctx)
        {
            var _logAction = "AddCommands_Ping";

            await ctx.TriggerTypingAsync();

            _logger.WriteEntry(new logging.Models.LogEntry
            {
                Action = _logAction,
                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                Message = $"Adding Command: {_logAction}"
            });

            // let's make the message a bit more colourful
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            // respond with ping
            await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");
        }
    }
}
