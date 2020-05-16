using DSharpPlus.EventArgs;
using System.Linq;
using System.Threading.Tasks;

namespace sctm.discordbot.Commands
{
    public partial class AttachmentCommands
    {
        public async Task ProcessImage(ulong id, MessageCreateEventArgs e)
        {
            var _logAction = "ProcessImage";
            var _attachment = e.Message.Attachments.Where(i => i.Id == id).FirstOrDefault();
            if(_attachment == null)
            {
                _logger.WriteEntry(new logging.Models.LogEntry
                {
                    Action = _logAction,
                    Level = Microsoft.Extensions.Logging.LogLevel.Error,
                    Message = $"Unable to find attachment on this message with Id: {id}"
                });
                return;
            }

            var _parseResult = await _services.SubmitScreenshotForParsing(_attachment.Url);
            if(_parseResult != null && _parseResult.RecordId != null)
            {
                await e.Message.RespondAsync($"processed attachment {id.ToString()}: {_attachment.FileName} for {e.Message.Author.Username}...");
            } else
            {
                await e.Message.RespondAsync("That's a big nope");
            }
            
        }
    }
}
