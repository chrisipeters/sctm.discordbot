﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System;
using System.ComponentModel.Design;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace sctm.services.discordBot.Commands.Messages
{
    public partial class MessageCommands
    {
        /*
        [Command("hello")] // let's define this method as a command
        [Description("Link your Discord account to your SCTM account")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("link")] // alternative names for the command
        public async Task AddCommands_Hello(CommandContext ctx, [Description("hello email and code")]params string[] args)
        {
            #region Get client

            if (_services.GetSCTMClient() == null)
            {
                Log.Error("Unable to get SCTM Client");
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)ctx.Client, ":cry:"));
            }

            #endregion

            await ctx.TriggerTypingAsync();

            var _values = ctx.Message.Content;

            #region Get client

            if (_services.GetSCTMClient() == null)
            {
                Log.Error("Unable to get SCTM Client");
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client,":cry:"));
            }

            #endregion

            var _req = new DiscordHelloRequest_POST
            {
                Code = args[1],
                Email = args[0],
                DiscordName = ctx.Message.Author.Username,
                DiscordDiscriminator = ctx.Message.Author.Discriminator,
                DiscordId = ctx.Message.Author.Id
            };

            StringContent _content = new StringContent(JsonConvert.SerializeObject(_req), Encoding.UTF8, "application/json");


            var _url = _config["SCTM:Urls:DiscordHello"];
            var _result = await (await _services.GetSCTMClient()).PostAsync(_url, _content);
            if (_result.IsSuccessStatusCode)
            {
                await ctx.RespondAsync("Done. You're all set!");
            }
            else
            {
                await ctx.RespondAsync("Sorry, something just went wrong: " + (await _result.Content.ReadAsStringAsync()));
            }
        }
        */
    }

    public class DiscordHelloRequest_POST
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public ulong? DiscordId { get; set; }
        public string DiscordName { get; set; }
        public string DiscordDiscriminator { get; set; }
    }
}
