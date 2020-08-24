using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using sctm.connectors.sctmDB.Models.DBModels.Screenshots.OCR.TradeConsole;

namespace sctm.services.discordBot
{
    public partial class Embeds
    {
        public static DiscordEmbed ConfirmScreen(ConfirmScreen data, MessageCreateEventArgs e, DiscordAttachment attachment, int recordId)
        {
            var _userName = e.Author.Username;
            var _userDiscriminator = e.Author.Discriminator;
            var _userAvatarUrl = e.Author.AvatarUrl;
            var _userId = e.Author.Id;
            var _channelName = (e.Channel?.Name != null && e.Channel?.Name.Trim().Length > 0) ? e.Channel?.Name : "Direct User";
            var _channelId = e.Channel.Id;
            var _guildName = (e.Guild?.Name != null) ? e.Guild.Name : "Direct User";
            //var _guildId = e.Guild.Id;

            var _ret = new DiscordEmbedBuilder
            {
                Title = $"Refinery Sale Completed by {_userName}",
                Description = $"**{_userName}** has completed a refinery sale worth **{data.TotalTransactionCost}**aUEC. **{data.TotalTransactionCost}**xp has been awarded.",
                ThumbnailUrl = _userAvatarUrl,
                ImageUrl = attachment.Url,
                Color = DiscordColor.Yellow,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = $"Star Citizen Tools by SC TradeMasters >> ConfirmScreen:{recordId}>>CreateScreen:{data.CreateScreenId}", IconUrl = e.Client.CurrentUser.AvatarUrl }
            }
            //.AddField($"**{_guildName}**", ":first_place:**Rank 3** [**1.2B**xp]")
            //.AddField($"**{_channelName}**", ":second_place:**Rank 27** [**1M**xp]")
            //.AddField($"**{_userName}#{_userDiscriminator}**", ":trophy:**Rank 1** [**27,324**xp]")
            .AddField($"Ship", data.ShipIdentifier, true)
            .AddField($"Total Value", $"**{data.TotalTransactionCost}** aUEC", true)
            ;


            return _ret;
        }
    }
}
