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
        public void AddCommands_Hello()
        {
            var _logAction = "AddCommands_Hello";

            _logger.WriteEntry(new logging.Models.LogEntry
            {
                Action = _logAction,
                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                Message = $"Adding Command: {_logAction}"
            });

            _discordClient.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower().StartsWith($"{_commandPreface} hello"))
                {
                    var c = await _discordClient.CreateDmAsync(e.Message.Author);

                    var _code = "dsadsada";

                    var _data = e.Message.Content.ToLower().Replace("~roo hello", "").Trim();
                    if(_data.Length == 0)
                    {
                        await c.SendMessageAsync($"To join Discord with your SCTradeMasters account I need you to reply in the following format: **{_commandPreface} hello *yourEmail@domain.com {_code}* **" );
                    } else
                    {
                        var _string = _data.Split(' ');
                        if(_string.Length == 2)
                        {
                            _logger.WriteEntry(new logging.Models.LogEntry
                            {
                                Action = _logAction,
                                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                                Message = $"Processing message: {e.Message.Content}"
                            });


                            await c.SendMessageAsync($"Processing hello code: {_code} for: {_string[0]}");
                        } else
                        {
                            await c.SendMessageAsync($"To join Discord with your SCTradeMasters account I need you to reply in the following format: **{_commandPreface} hello *yourEmail@domain.com {_code}* **");
                        }
                    }
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
