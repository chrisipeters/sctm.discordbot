﻿using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Threading;

namespace sctm.services.discordBot
{
    public partial class Embeds
    {
        public static DiscordEmbed Register(DiscordMessage e, string botAvatarUrl)
        {
            

            var _userName = e.Author.Username;
            var _userDiscriminator = e.Author.Discriminator;
            var _userAvatarUrl = e.Author.AvatarUrl;
            var _userId = e.Author.Id;
            var _channelName = e.Channel.Name;
            var _channelId = e.Channel.Id;
            var _serverName = e.Channel.Guild.Name;
            var _serverAvatarUrl = e.Channel.Guild.IconUrl;
            var _serverId = e.Channel.Guild.Id;

            var _registerUrl = $"https://sctrademasters.com/register?ref=discord&server={_serverId}";

            var _ret = new DiscordEmbedBuilder
            {
                Title = $"{_serverName} Tools and Leaderboards",
                Description = $"As a member of {_serverName}, we're excited to invite you for early-access to Profession Leaderboards and Gameplay Tools for Star Citizen. [Click here]({_registerUrl}) to access the register page.\n\n We're still in heavy development, but here's what's coming:",
                ThumbnailUrl = _serverAvatarUrl,
                Color = DiscordColor.Red,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Star Citizen Tools by SC TradeMasters | Season 1 will end 1 June, 2020", IconUrl = botAvatarUrl }
            }
            .AddField(":pick: Mining *-alpha launching soon*", "**Mining experience & leaderboards** - Whether you're looking to level-up your mining skills, or compete with players across the verse in our leaderboards - this is the solution for you!")
            .AddField(":dollar: Trade *-In development*", "**Trade Resources & leaderboards** - If you're looking for resources to help make trade more profitable, or ready to compete against the titans of trade, our Trade and Economy community is the place for you.")
            .AddField(":checkered_flag: Racing *-in development*", "If you're looking to keep up with you or your org's movement on the leaderboards - or looking to setup private competitions; our racing community is the place for you")
            .AddField(":gun: Combat *-in development*", "Keep track of your position on the RSI leaderboards and schedule matches between your team and others. Our combat community gives you the place to show off your FPS and Ship combat skills")
            .AddField(":computer: Register now", $"[Click here]({_registerUrl}) to join today for free!")
            ;


            return _ret;
        }
    }
}
