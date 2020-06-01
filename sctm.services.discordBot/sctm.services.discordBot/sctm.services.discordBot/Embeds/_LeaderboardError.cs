using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sctm.services.discordBot
{
    public partial class Embeds
    {
        public static DiscordEmbed LeaderboardError(MessageCreateEventArgs e, DiscordAttachment attachment, string error)
        {
            var _userAvatarUrl = e.Author.AvatarUrl;

            LeaderboardErrorMessages _error = null; 
            try { _error = JsonConvert.DeserializeObject<LeaderboardErrorMessages>(error); }
            catch (Exception ex)
            {
                var f = ex.Message;
                _error = null;
            }

            string _description = "My apologies, I was unable to process your image. Here's why:\n";
            if (_error == null) _description += error;
            else
            {
                if (_error.Image != null && _error.Image.Any())
                {
                    _description += "**Image Errors:**\n";
                    foreach (var item in _error.Image) { _description += $"{item}\n"; }
                }

                if (_error.Parser != null && _error.Parser.Any())
                {
                    _description += "**Parser Errors:**\n";
                    foreach (var item in _error.Parser) { _description += $"{item}\n"; }
                }

            }

            var _ret = new DiscordEmbedBuilder
            {
                Title = $"Unable to process image",
                Description = (_error == null) ? _description : "My apologies, I was unable to process your image. Here's why:",
                ThumbnailUrl = _userAvatarUrl,
                ImageUrl = attachment.Url,
                Color = DiscordColor.IndianRed,
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = $"Star Citizen Tools by SC TradeMasters", IconUrl = e.Client.CurrentUser.AvatarUrl }
            };

            if(_error != null)
            {
                if(_error.Image != null && _error.Image.Any()) _ret.AddField("Image Errors", string.Join("\n", _error.Image));
                if (_error.Parser != null && _error.Parser.Any()) _ret.AddField("Parser Errors", string.Join("\n", _error.Parser));
            }

            return _ret;
        }
    }

    public class LeaderboardErrorMessages
    {
        public List<string> Image { get; set; }
        public List<string> Parser { get; set; }
    }
}
