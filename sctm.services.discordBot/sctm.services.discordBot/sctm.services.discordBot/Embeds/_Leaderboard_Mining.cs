using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using sctm.services.discordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sctm.services.discordBot
{
    public partial class Embeds
    {
        public static async Task<DiscordEmbed> Leaderboard_Mining(CommandContext ctx, DiscordUser bot, Leaderboards_Result leaderboard)
        {
            if (leaderboard?.Results?.Professions == null) return null;

            var _profession = leaderboard?.Results?.Professions?.Where(i => i.Name.ToLower() == "mining").FirstOrDefault();
            if (_profession == null) return null;

            var _teams = _profession.Entrants.OrderByDescending(i => i.EarnedCredits);

            var _topPlayers = new List<(DiscordMember dUser, string team, ulong xp, ulong credits)>();
            foreach (var team in _teams)
            {
                foreach (var player in team.TopPlayers)
                {
                    _topPlayers.Add((
                        await ctx.Guild.GetMemberAsync(ulong.Parse(player.Name)),
                        ctx.Guild.GetChannel(ulong.Parse(team.Name)).Name,
                        player.Amount,
                        player.EarnedCredits
                        ));
                }
            }

            var _duration = (leaderboard.Season.Dates.End - DateTime.Now);

            var _timeLeft = (_duration.TotalDays >= 2) ? $"{Math.Round(_duration.TotalDays,0)} days" :$"{Math.Round(_duration.TotalHours,1)} hours";

            var _news = $"Season {leaderboard.Season.Number} - *{leaderboard.Season.Name}* is underway and running until: {leaderboard.Season.Dates.End.ToUniversalTime().ToString("dd MMM")} UTC.";

            if(_teams != null && _teams.Any())
            {
                try
                {
                    var _topTeam = ctx.Guild.GetChannel(ulong.Parse(_teams.OrderByDescending(i => i.EarnedCredits).Select(i => i.Name).First()));
                    var _topTeamPlayer = _topPlayers.Where(i => i.team == _topTeam.Name).OrderByDescending(i => i.credits).First();

                    _news += $"\n\n:newspaper: Currently **{_topTeam.Name}** lead by *{_topTeamPlayer.dUser.Username}* is on top with {_timeLeft} left to go...";
                }
                catch (Exception ex)
                {
                    // logging
                }
            }


            var _ret = new DiscordEmbedBuilder
            {
                Title = $"{ctx.Guild.Name} - :pick: Mining",
                Description = _news,
                ImageUrl = "https://media.robertsspaceindustries.com/wo06flt432pjs/store_small.jpg",
                ThumbnailUrl = _topPlayers.Select(i => i.dUser.AvatarUrl).FirstOrDefault(),
                Color = DiscordColor.Green,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = $"Star Citizen Tools by SC TradeMasters", IconUrl = bot.AvatarUrl }
            };

            #region Team Placements


            var _i = 0;

            if (_teams != null && _teams.Any()) foreach (var team in _teams.OrderByDescending(i => i.EarnedCredits))
                {
                    string _emoji = ":first_place:";

                    if (_i == 0) _emoji = ":first_place:";
                    else if (_i == 1) _emoji = ":second_place:";
                    else if (_i == 2) _emoji = ":third_place:";
                    else _emoji = ":medal:";

                    _ret.AddField(
                $"{_emoji} {ctx.Guild.GetChannel(ulong.Parse(team.Name)).Name} - {team.TotalPlayers}:adult: {team.TotalRecords}:receipt:",
                $" {team.EarnedCredits}:moneybag: | {team.Amount}:muscle:");

                    _i += 1;
                }

            #endregion

            #region Player placements

            _i = 0;
            var _playerString = "";
            if (_topPlayers != null && _topPlayers.Any())
            {
                foreach (var player in _topPlayers.OrderByDescending(i => i.credits))
                {
                    string _emoji = ":first_place:";

                    if (_i == 0) _emoji = ":first_place:";
                    else if (_i == 1) _emoji = ":second_place:";
                    else if (_i == 2) _emoji = ":third_place:";
                    else _emoji = ":medal:";

                    _playerString += $"{_emoji} **{player.dUser.Username}** *{player.team}*\n {player.credits}:moneybag: {player.xp}:muscle:\n";

                    _i += 1;
                }

                _ret.AddField("Top Players", _playerString);
            }


            #endregion

            return _ret;
        }
    }
}
