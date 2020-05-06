using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Linq;

namespace sctm.discordbot
{
    public partial class Embeds
    {
        public static DiscordEmbed RefinerySellConfirm(connectors.azureComputerVision.models.Terminals.Refinery.Confirm data, MessageCreateEventArgs e, DiscordAttachment attachment, string avatarUrl)
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
                Title = $"Refinery Sale Completed by {_userName}",
                Description = $"**{_userName}** has completed a refinery sale worth **{data.TotalTransactionCost}**aUEC. **{data.TotalTransactionCost}**xp has been awarded.",
                ThumbnailUrl = _userAvatarUrl,
                ImageUrl = attachment.Url,
                Color = DiscordColor.Yellow,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Star Citizen Tools by SC TradeMasters | Season 1 will end 1 June, 2020", IconUrl = avatarUrl }
            }
            .AddField($"**{_guildName}**", ":first_place:**Rank 3** [**1.2B**xp]")
            .AddField($"**{_channelName}**", ":second_place:**Rank 27** [**1M**xp]")
            .AddField($"**{_userName}#{_userDiscriminator}**", ":trophy:**Rank 1** [**27,324**xp]")
            .AddField($"Ship", data.ShipIdentifier, true)
            .AddField($"Total Value", $"**{data.TotalTransactionCost}** aUEC", true)
            ;


            return _ret;
        }
    }
}
