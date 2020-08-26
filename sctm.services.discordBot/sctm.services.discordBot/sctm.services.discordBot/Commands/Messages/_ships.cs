using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using sctm.connectors.rsi.models;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sctm.services.discordBot.Commands.Messages
{
    public partial class MessageCommands
    {
        [Command("ships")] // let's define this method as a command
        [Description("Get information about a ship in Star Citizen")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("ship")] // alternative names for the command
        public async Task AddCommands_Ship(CommandContext ctx, [Description("Search term")] params string[] args)
        {
            var _logAction = "AddCommands_Ship";
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":mag:"));

            var _client = await _services.GetSCTMClient();
            if (_client == null)
            {
                Log.Error("{logAction}: unable to get SCTM client", _logAction);
                await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromName(ctx.Client, ":mag:"));
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":cry:"));
                return;
            }

            string _searchTerm = null;
            if (args != null && args.Any()) _searchTerm = string.Join(' ', args);

            var _url = _config["SCTM:Urls:Ships"];
            if (_searchTerm != null) _url += $"?search={_searchTerm}";



            var _request = await _client.GetAsync(_url);
            var _content = await _request.Content.ReadAsStringAsync();

            await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromName(ctx.Client, ":mag:"));

            if (_request.IsSuccessStatusCode)
            {
                
                var _ships = JsonConvert.DeserializeObject<List<Ship>>(_content);

                foreach (var ship in _ships)
                {
                    var _embed = Embeds.Ship(ship, ctx.Client.CurrentUser);
                    await ctx.Message.RespondAsync(null, false, _embed);
                }
            } else
            {
                Log.Error("{logAction}: unsuccessful API call: {@content}", _logAction, _content);
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":cry:"));
                return;
            }
        }
    }
}
