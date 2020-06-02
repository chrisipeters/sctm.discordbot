using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
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
            #region Get client

            if (_services.GetSCTMClient() == null)
            {
                _logger.LogError("Unable to get SCTM Client");
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)ctx.Client, ":cry:"));
                return;
            }

            #endregion

            var _url = _config["SCTM:Urls:GetRSILeaderboard"].TrimEnd('/') + $"/{args[0].Trim()}";

            var _result = (await _services.GetSCTMClient()).GetAsync(_url);
            
        }

        private async Task<HttpResponseMessage> MakeCall(HttpClient client, string url, MultipartFormDataContent content)
        {

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode) return response;
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // 401 
                // Authorization header has been set, but the server reports that it is missing.
                // It was probably stripped out due to a redirect.

                var finalRequestUri = response.RequestMessage.RequestUri;

                if (finalRequestUri != new Uri(url)) return await MakeCall(client, finalRequestUri.ToString(), content);
                else return response;
            }
            else return response;
        }
    }
}
