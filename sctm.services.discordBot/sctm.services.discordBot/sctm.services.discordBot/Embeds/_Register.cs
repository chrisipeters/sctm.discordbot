using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Threading;

namespace sctm.services.discordBot
{
    public partial class Embeds
    {
        public static DiscordEmbed Register(MessageCreateEventArgs e)
        {
            string serverName = (e.Message.Channel.Guild?.Name != null) ? e.Message.Channel.Guild.Name : "SCTradeMasters";
            string serverAvatarUrl = (e.Message.Channel.Guild?.IconUrl != null) ? e.Message.Channel.Guild.IconUrl : e.Client.CurrentUser.AvatarUrl;
            ulong serverId = (e.Message.Channel.Guild?.Id != null) ? e.Message.Channel.Guild.Id : e.Client.CurrentUser.Id;
            string botAvatarUrl = e.Client.CurrentUser.AvatarUrl;

            return Register(serverName, serverAvatarUrl, serverId, botAvatarUrl);
        }
        public static DiscordEmbed Register(string serverName, string serverAvatarUrl, ulong serverId, string botAvatarUrl)
        {

            var _registerUrl = $"https://sctrademasters.com/register?ref=discord&server={serverId}";

            var _ret = new DiscordEmbedBuilder
            {
                Title = $"{serverName} Tools and Leaderboards",
                Description = $"As a member of {serverName}, we're excited to invite you for early-access to Profession Leaderboards and Gameplay Tools for Star Citizen. [Click here]({_registerUrl}) to access the register page.\n\n We're still in heavy development, but here's what's coming:",
                ThumbnailUrl = serverAvatarUrl,
                Color = DiscordColor.Red,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Star Citizen Tools by SC TradeMasters", IconUrl = botAvatarUrl }
            }
            .AddField(":pick: Mining *-alpha launching soon*", "**Mining experience & leaderboards** - Whether you're looking to level-up your mining skills, or compete with players across the verse in our leaderboards - this is the solution for you!")
            .AddField(":dollar: Trade *-In development*", "**Trade Resources & leaderboards** - If you're looking for resources to help make trade more profitable, or ready to compete against the titans of trade, our Trade and Economy community is the place for you.")
            .AddField(":checkered_flag: Racing *-in development*", "If you're looking to keep up with you or your org's movement on the leaderboards - or looking to setup private competitions; our racing community is the place for you")
            .AddField(":gun: Combat *-in development*", "Keep track of your position on the RSI leaderboards and schedule matches between your team and others. Our combat community gives you the place to show off your FPS and Ship combat skills")
            .AddField(":computer: Register now", $"[Click here]({_registerUrl}) to join today for free!")
            ;


            return _ret;
        }
    }
}
