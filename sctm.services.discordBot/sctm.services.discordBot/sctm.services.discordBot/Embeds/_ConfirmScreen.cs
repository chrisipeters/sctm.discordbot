using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using sctm.services.discordBot.Models;
using System;
using System.Threading.Tasks;

namespace sctm.services.discordBot
{
    public partial class Embeds
    {
        public static async Task<DiscordEmbed> ConfirmScreen(TradingConsole_POST_Result data, MessageCreateEventArgs e, DiscordAttachment attachment)
        {
            var _record = JsonConvert.DeserializeObject<AddConfirmScreen_Record>(data.Record.ToString());

            var _userName = e.Author.Username;
            var _userDiscriminator = e.Author.Discriminator;
            var _userAvatarUrl = e.Author.AvatarUrl;
            var _userId = e.Author.Id;
            var _channelName = (e.Channel?.Name != null && e.Channel?.Name.Trim().Length > 0) ? e.Channel?.Name : "Direct User";
            var _channelId = e.Channel.Id;
            var _guildName = (e.Guild?.Name != null) ? e.Guild.Name : "Direct User";
            //var _guildId = e.Guild.Id;

            DiscordEmbedBuilder _ret = null;

            try
            {
                _ret = new DiscordEmbedBuilder
                {
                    Title = $"{_record.TransactionType.ToString().ToUpper()} action confirmed by {_userName}",
                    Description = $"**{_userName}** has confirmed a {_record.TransactionType.ToString().ToUpper()} action of {_record.Units} units worth **{_record.TotalTransactionCost}**aUEC. The following experience has been awarded:\n\n",
                    ThumbnailUrl = _userAvatarUrl,
                    ImageUrl = attachment.Url,
                    Color = DiscordColor.Green,
                    Footer = new DiscordEmbedBuilder.EmbedFooter { Text = $"SCTradeMasters.com >> ConfirmScreen:{_record.Id}>>CreateScreen:{(_record.CreateScreenId ?? 0)}", IconUrl = e.Client.CurrentUser.AvatarUrl }
                }
                .AddField($"Org: {e.Message.Channel.Guild.Name}", $"Team: {e.Message.Channel.Name}", false)
                .AddField($":rocket: {_record.ShipIdentifier}", $"{data.Experience?.Ship.New.ShipXP.ToString() ?? "No awarded "}xp (+{data.Experience?.Ship.Entry.Awarded.ToString() ?? "0"})", true);

                if (data?.Experience?.Professions != null) foreach (var prof in data.Experience.Professions)
                    {
                        _ret.AddField($":pick: {prof.Entry.Profession}", $"{prof.Entry.Proficiency} - {prof.New.ProfessionXP}xp (+{prof.Entry.Awarded})", true);
                    }

                if (data?.Experience?.Tasks != null)
                {

                    string _taskString = "";
                    foreach (var task in data.Experience.Tasks)
                    {
                        _taskString += $"**{task.Entry.Task}**: {task.New.TaskXP}xp (+{task.Entry.Awarded})\n";
                    }

                    _ret.AddField(":gear: Tasks", _taskString, false);
                }
            }
            catch (Exception ex)
            {
                //logging
                _ret = null;
            }




            return _ret;
        }
    }
}
