using DSharpPlus;
using Microsoft.Extensions.Configuration;
using sctm.connectors.azureComputerVision;
using sctm.logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace sctm.discordbot
{
    public partial class Screenshots
    {
        private HttpClient _client;
        private IConfiguration _config;
        private DiscordClient _discord;
        private FileInfo _logFile;
        private FileLogger _logger;
        private AzureComputerVisionRepository _repoComputerVision;

        public Screenshots(IConfiguration config, DiscordClient discord, FileInfo logFile)
        {
            _config = config;
            _discord = discord;
            _logFile = logFile;
            _logger = new FileLogger($"discord-{_discord.CurrentUser.Username}", Environment.MachineName, "Screenshots", _logFile.FullName);
            _repoComputerVision = new AzureComputerVisionRepository(new AzureConfiguration
            {
                Endpoint = _config["Azure:Endpoint"],
                SubscriptionKey = _config["Azure:SubscriptionKey"]
            });
        }

        public async void StartListening()
        {

            var _logAction = "StartListening";

            _discord.MessageCreated += async e =>
            {
                var _messageId = e.Message.Id;
                var _userId = e.Author.Id;
                var _channelName = e.Channel.Name;
                var _channelId = e.Channel.Id;
                var _guildName = e.Guild.Name;
                var _guildId = e.Guild.Id;
                var _botAvatar = _discord.CurrentUser.AvatarUrl;



                if (e.Message.Attachments != null && e.Message.Attachments.Any())
                {
                    foreach (var item in e.Message.Attachments)
                    {
                        _logger.WriteEntry(new logging.Models.LogEntry
                        {
                            Action = _logAction,
                            Level = Microsoft.Extensions.Logging.LogLevel.Information,
                            Message = $"Processing message attachment for {e.Author.Username}#{e.Author.Discriminator}"
                        });

                        await e.Message.RespondAsync($"processing: {item.FileName} for {e.Message.Author.Username}...");

                        _logger.WriteEntry(new logging.Models.LogEntry
                        {
                            Action = _logAction,
                            Level = Microsoft.Extensions.Logging.LogLevel.Information,
                            Message = $"Getting image from Url: {item.Url}"
                        });

                        if (!item.Url.ToLower().EndsWith(".jpg"))
                        {
                            _logger.WriteEntry(new logging.Models.LogEntry
                            {
                                Action = _logAction,
                                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                                Message = $"Invalid file format"
                            });

                            await e.Message.RespondAsync("Crikey! I was expecting a jpg here. Not much I can do with that file");
                        }
                        else
                        {
                            var _res = await _client.GetAsync(item.Url);

                            _logger.WriteEntry(new logging.Models.LogEntry
                            {
                                Action = _logAction,
                                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                                Message = $"Reading image to steam"
                            });

                            var _stream = await _res.Content.ReadAsStreamAsync();

                            //create new MemoryStream object
                            MemoryStream memStream = new MemoryStream();
                            memStream.SetLength(_stream.Length);
                            //read file to MemoryStream
                            _stream.Read(memStream.GetBuffer(), 0, (int)_stream.Length);

                            _logger.WriteEntry(new logging.Models.LogEntry
                            {
                                Action = _logAction,
                                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                                Message = $"Uploading image with connector"
                            });

                            var _uploadResult = await _repoComputerVision.Upload(memStream);

                            _logger.WriteEntry(new logging.Models.LogEntry
                            {
                                Action = _logAction,
                                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                                Message = $"Processing image as {_uploadResult.type.ToString()}"
                            });

                            switch (_uploadResult.type)
                            {
                                case connectors.azureComputerVision.models.ScreenShotTypes.Unknown:
                                    break;
                                case connectors.azureComputerVision.models.ScreenShotTypes.TradeConsole_BUY:
                                    break;
                                case connectors.azureComputerVision.models.ScreenShotTypes.TradeConsole_SELL:
                                    break;
                                case connectors.azureComputerVision.models.ScreenShotTypes.TradeConsole_BUYConfirm:
                                    var _dataTradeBuyConfirm = (connectors.azureComputerVision.models.Terminals.Trade.Confirm)_uploadResult.result;
                                    await e.Message.RespondAsync(embed: Embeds.TradeBUYConfirm(_dataTradeBuyConfirm, e, item, _botAvatar));
                                    break;
                                case connectors.azureComputerVision.models.ScreenShotTypes.TradeConsole_SELLConfirm:
                                    var _dataTradeSellConfirm = (connectors.azureComputerVision.models.Terminals.Trade.Confirm)_uploadResult.result;
                                    await e.Message.RespondAsync(embed: Embeds.TradeSELLConfirm(_dataTradeSellConfirm, e, item, _botAvatar));
                                    break;
                                case connectors.azureComputerVision.models.ScreenShotTypes.FleetManager:
                                    break;
                                case connectors.azureComputerVision.models.ScreenShotTypes.RefineryTerminal_SELL:
                                    var _dataRefinerySell = (connectors.azureComputerVision.models.Terminals.Refinery.Sell)_uploadResult.result;
                                    await e.Message.RespondAsync(embed: Embeds.RefinerySellEmbed(_dataRefinerySell, e, item, _botAvatar));
                                    break;
                                case connectors.azureComputerVision.models.ScreenShotTypes.RefineryTerminal_SELLConfirm:
                                    var _dataRefineryConfirm = (connectors.azureComputerVision.models.Terminals.Refinery.Confirm)_uploadResult.result;
                                    await e.Message.RespondAsync(embed: Embeds.RefinerySellConfirm(_dataRefineryConfirm, e, item, _botAvatar));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                }
            };
        }
    }
}
