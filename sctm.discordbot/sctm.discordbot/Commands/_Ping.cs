using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using sctm.connectors.azureComputerVision;
using sctm.logging;
using System.Net.Http;

namespace sctm.discordbot
{
    public partial class Commands
    {
        public void AddCommands_Ping()
        {
            var _logAction = "AddCommands_Ping";

            _logger.WriteEntry(new logging.Models.LogEntry
            {
                Action = _logAction,
                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                Message = $"Adding Command: {_logAction}"
            });

            _discordClient.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower().StartsWith($"{_commandPreface} ping"))
                {
                    _logger.WriteEntry(new logging.Models.LogEntry
                    {
                        Action = _logAction,
                        Level = Microsoft.Extensions.Logging.LogLevel.Information,
                        Message = $"Processing message: {e.Message.Content}"
                    });

                    await e.Message.RespondAsync("pong!");
                }
            };

            _logger.WriteEntry(new logging.Models.LogEntry
            {
                Action = _logAction,
                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                Message = $"Command: {_logAction} Added"
            });
        }
    }
}
