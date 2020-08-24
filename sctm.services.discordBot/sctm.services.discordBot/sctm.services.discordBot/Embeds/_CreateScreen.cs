using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using sctm.connectors.sctmDB.Models.DBModels.Screenshots.OCR.TradeConsole;
using System;

namespace sctm.services.discordBot
{
    public partial class Embeds
    {
        public static DiscordEmbed CreateScreen(CreateScreen data, MessageCreateEventArgs e, DiscordAttachment attachment, int recordId)
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
                Title = $"New Refinery Sale Option by {_userName}",
                Description = $"**{_userName}** is showing an option to sell **{data.TotalCargoSpace - data.EmptyCargoSpace}**{data.CargoUnitOfMeasure.ToString()} of goods for a gross profit of **{data.TotalTransactionValue}**aUEC.\nUpon completing this sale **{data.TotalTransactionValue}**xp will be awarded to the player, team, and org.",
                ThumbnailUrl = _userAvatarUrl,
                ImageUrl = attachment.Url,
                Color = DiscordColor.Yellow,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = $"Star Citizen Tools by SC TradeMasters >> CreateScreen:{recordId}", IconUrl = e.Client.CurrentUser.AvatarUrl }
            }
            .AddField("Organization",$"**{_guildName}**", true)
            .AddField("Team", $"**{_channelName}**", true)
            //.AddField($"**{_userName}#{_userDiscriminator}**", ":trophy:**Rank 1** [**27,324**xp]")
            .AddField($"Ship", data.ShipIdentifier)
            .AddField($"Total Value *({data.Items.Count} item types)*", $"**{data.TotalTransactionValue}** aUEC")
            ;

            foreach (var item in data.Items)
            {
                _ret.AddField($"**{item.Name}** ({Math.Round(item.Units / (data.TotalCargoSpace - data.EmptyCargoSpace),2)}%)", $"{item.Units}{data.CargoUnitOfMeasure.ToString()} @ {item.PricePerUnit} = {item.LoadValue}", true);
            }


            return _ret;
        }
    }
}
