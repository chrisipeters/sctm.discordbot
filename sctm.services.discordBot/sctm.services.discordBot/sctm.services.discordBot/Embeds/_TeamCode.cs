using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace sctm.services.discordBot
{
    public partial class Embeds
    {
        public static DiscordEmbed TeamCode(MessageCreateEventArgs e)
        {
            string serverName = (e.Message.Channel.Guild?.Name != null) ? e.Message.Channel.Guild.Name : "SCTradeMasters";
            string channelName = (e.Message.Channel.Name != null) ? e.Message.Channel.Name : "Unknown Team";
            string channelId = e.Message.Channel.Id.ToString();
            string serverAvatarUrl = (e.Message.Channel.Guild?.IconUrl != null) ? e.Message.Channel.Guild.IconUrl : e.Client.CurrentUser.AvatarUrl;
            string botAvatarUrl = e.Client.CurrentUser.AvatarUrl;

            return TeamCode(serverName, channelName, channelId, serverAvatarUrl, botAvatarUrl);
        }

        public static DiscordEmbed TeamCode(CommandContext ctx)
        {
            string serverName = (ctx.Message.Channel.Guild?.Name != null) ? ctx.Message.Channel.Guild.Name : "SCTradeMasters";
            string channelName = (ctx.Message.Channel.Name != null) ? ctx.Message.Channel.Name : "Unknown Team";
            string channelId = ctx.Message.Channel.Id.ToString();
            string serverAvatarUrl = (ctx.Message.Channel.Guild?.IconUrl != null) ? ctx.Message.Channel.Guild.IconUrl : ctx.Client.CurrentUser.AvatarUrl;
            string botAvatarUrl = ctx.Client.CurrentUser.AvatarUrl;

            return TeamCode(serverName, channelName, channelId, serverAvatarUrl, botAvatarUrl);
        }

        public static DiscordEmbed TeamCode(string serverName, string channelName, string channelId, string serverAvatarUrl, string botAvatarUrl)
        {
            

            var _ret = new DiscordEmbedBuilder
            {
                Title = $"Welcome to Team: {serverName} > {channelName}",
                Description = $"We use your Discord Channel to determine the Team you are competing for. As part of this channel you are competing for:",
                ThumbnailUrl = serverAvatarUrl,
                Color = DiscordColor.Purple,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Star Citizen Tools by SC TradeMasters", IconUrl = botAvatarUrl }
            }
            .AddField("Organization", serverName, true)
            .AddField("Team", channelName, true)
            .AddField("Team Code", channelId, true)
            ;

            return _ret;
        }
    }
}
