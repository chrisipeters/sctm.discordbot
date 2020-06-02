using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using sctm.connectors.rsi.models.Leaderboards;
using sctm.services.discordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sctm.services.discordBot
{
    public partial class Embeds
    {
        public static DiscordEmbed RSILeaderboard(DiscordGuild guild, DiscordUser orgChampion, DiscordUser bot, LeaderboardInsight leaderboard, Leaderboard prevLeaderboard, DateTime dataDate)
        {
            var _seenMembers = new Dictionary<string, int>();
            foreach (var map in leaderboard.Maps.Select(i => i.Value.current))
            {
                foreach (var entry in map.Results) _seenMembers[entry.Nickname] = 0;
            }

            foreach (var map in leaderboard.Maps.Select(i => i.Value.current))
            {
                foreach (var entry in map.Results) _seenMembers[entry.Nickname] += (100 - entry.Rank);
            }



            var _ret = new DiscordEmbedBuilder
            {
                Title = $":checkered_flag: {leaderboard.Name} results - {dataDate.ToString("d MMM - htt")}",
                Description = $@"
                    Here at The Corporation over the past hour:\n
                    - Greatest Improvement: **OldVandeval** Up 2 places\n
                    - Largest dropoff: **RikkordMemorialRaceway** down 6 spots.\n\n
                    @chrispykoala has been our overall champion for **3 hours in a row!**",
                ThumbnailUrl = orgChampion.AvatarUrl,
                ImageUrl = leaderboard.ImageUrl,
                Color = DiscordColor.Yellow,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = $"Star Citizen Tools by SC TradeMasters", IconUrl = bot.AvatarUrl }
            };


            return _ret;
        }
    }
}
