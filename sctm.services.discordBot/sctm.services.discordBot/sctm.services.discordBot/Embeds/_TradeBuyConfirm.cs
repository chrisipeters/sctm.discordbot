using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace sctm.services.discordBot
{
    public partial class Embeds
    {
        public static DiscordEmbed TradeBUYConfirm(sctm.connectors.azureComputerVision.models.Terminals.Trade.Confirm data, MessageCreateEventArgs e, DiscordAttachment attachment, string recordId)
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
                Title = $"New Trade Console BUY Completed by {_userName}",
                Description = $"**{_userName}** has purchased {data.Item.Quantity} units of {data.Item.Name} for a total transaction cost of {data.Item.TransactionCost}.",
                ThumbnailUrl = _userAvatarUrl,
                ImageUrl = attachment.Url,
                Color = DiscordColor.Blue,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = $"Star Citizen Tools by SC TradeMasters >> Terminals_TradeBuyConfirm:{recordId}", IconUrl = e.Client.CurrentUser.AvatarUrl }
            }
            .AddField($"**{_guildName}**", ":first_place:**Rank 3** [**1.2B**xp]")
            .AddField($"**{_channelName}**", ":second_place:**Rank 27** [**1M**xp]")
            .AddField($"**{_userName}#{_userDiscriminator}**", ":trophy:**Rank 1** [**27,324**xp]")
            .AddField($"Ship", data.ShipIdentifier)
            .AddField($"Purchased {data.Item.Name}", $"{data.Item.Quantity} @ {data.Item.PricePerUnit} = {data.Item.TransactionCost}")
            ;

            return _ret;
        }
    }
}
