using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.IO;

namespace sctm.services.discordBot
{
    public partial class Embeds
    {
        public static DiscordEmbed RefinerySellEmbed(sctm.connectors.azureComputerVision.models.Terminals.Refinery.Sell data, MessageCreateEventArgs e, DiscordAttachment attachment, string recordId)
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
                Description = $"**{_userName}** is showing an option to sell **{data.UnrefinedCargo}**scu of mined materials to the refinery for a gross profit of **{data.TotalValue}**aUEC.\nUpon completing this sale **{data.TotalValue}**xp will be awarded to the player, team, and org.",
                ThumbnailUrl = _userAvatarUrl,
                ImageUrl = attachment.Url,
                Color = DiscordColor.Yellow,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = $"Star Citizen Tools by SC TradeMasters >> Terminals_RefinerySell:{recordId}", IconUrl = e.Client.CurrentUser.AvatarUrl }
            }
            .AddField($"**{_guildName}**", ":first_place:**Rank 3** [**1.2B**xp]")
            .AddField($"**{_channelName}**", ":second_place:**Rank 27** [**1M**xp]")
            .AddField($"**{_userName}#{_userDiscriminator}**", ":trophy:**Rank 1** [**27,324**xp]")
            .AddField($"Ship", data.ShipIdentifier)
            .AddField($"Total Value *({data.Items.Count} item types)*", $"**{data.TotalValue}** aUEC")
            ;

            foreach (var item in data.Items)
            {
                _ret.AddField($"**{item.Name}**", $"**{item.Value}** aUEC\n*({item.Amount} %)*", true);
            }


            return _ret;
        }
    }
}
