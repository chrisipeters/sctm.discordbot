using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using sctm.connectors.azureComputerVision.models.Terminals.Refinery;
using System;
using System.Linq;

namespace sctm.discordbot
{
    public partial class Embeds
    {
        public static DiscordEmbed TradeSELLConfirm(connectors.azureComputerVision.models.Terminals.Trade.Confirm data, MessageCreateEventArgs e, DiscordAttachment attachment, string avatarUrl)
        {
            var _userName = e.Author.Username;
            var _userDiscriminator = e.Author.Discriminator;
            var _userAvatarUrl = e.Author.AvatarUrl;
            var _userId = e.Author.Id;
            var _channelName = e.Channel.Name;
            var _channelId = e.Channel.Id;
            var _guildName = e.Guild.Name;
            var _guildId = e.Guild.Id;

            var _ret = new DiscordEmbedBuilder
            {
                Title = $"New Trade Console SELL Completed by {_userName}",
                Description = $"**{_userName}** has Sold {data.Item.Quantity} units of {data.Item.Name} for {data.Item.TransactionCost} netting a profit of **XXX** aUEC. **XXX**xp has been awarded",
                ThumbnailUrl = _userAvatarUrl,
                ImageUrl = attachment.Url,
                Color = DiscordColor.Blue,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Star Citizen Tools by SC TradeMasters | Season 1 will end 1 June, 2020", IconUrl = avatarUrl }
            }
            .AddField($"**{_guildName}**", ":first_place:**Rank 3** [**1.2B**xp]")
            .AddField($"**{_channelName}**", ":second_place:**Rank 27** [**1M**xp]")
            .AddField($"**{_userName}#{_userDiscriminator}**", ":trophy:**Rank 1** [**27,324**xp]")
            .AddField($"Ship", data.ShipIdentifier)
            .AddField($"Sold {data.Item.Name}", $"{data.Item.Quantity} @ {data.Item.PricePerUnit} = {data.Item.TransactionCost}")
            ;

            return _ret;
        }
    }
}
