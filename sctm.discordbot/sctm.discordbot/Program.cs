using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using sctm.connectors.azureComputerVision;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace sctm.discordbot
{
    class Program
    {
        private static HttpClient _client;
        static DiscordClient discord;

        public static IConfigurationRoot Configuration { get; private set; }

        static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
                .Build();

            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            _client = new HttpClient();
            DiscordEmbedBuilder _embed;



            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = Configuration["Discord:Token"],
                TokenType = TokenType.Bot
            });

            var _uploader = new AzureComputerVisionRepository(new AzureConfiguration
            {
                SubscriptionKey = Configuration["Azure:SubscriptionKey"],
                Endpoint = Configuration["Azure:Endpoint"]
            });

            discord.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower().StartsWith("ping"))
                    await e.Message.RespondAsync("pong!");
            };

            discord.MessageCreated += async e =>
            {
                var _userId = e.Author.Id;
                var _channelName = e.Channel.Name;
                var _channelId = e.Channel.Id;
                var _guildName = e.Guild.Name;
                var _guildId = e.Guild.Id;

                if (e.Message.Attachments != null && e.Message.Attachments.Any())
                {
                    foreach (var item in e.Message.Attachments)
                    {
                        await e.Message.RespondAsync($"processing: {item.FileName} for {e.Message.Author.Username}...");


                        var _res = await _client.GetAsync(item.Url);

                        var _stream = await _res.Content.ReadAsStreamAsync();

                        //create new MemoryStream object
                        MemoryStream memStream = new MemoryStream();
                        memStream.SetLength(_stream.Length);
                        //read file to MemoryStream
                        _stream.Read(memStream.GetBuffer(), 0, (int)_stream.Length);

                        var _uploadResult = await _uploader.Upload(memStream);

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

                                _embed = new DiscordEmbedBuilder
                                {
                                    ThumbnailUrl = item.Url,
                                    Color = DiscordColor.Blue,
                                    Title = "Processed Trade Terminal BUY Confirm screen",
                                }
                                .AddField("Member", $"{e.Author.Username}#{e.Author.Discriminator}", true)
                                .AddField($"Server", _guildName, true)
                                .AddField($"Channel", _channelName, true)
                                .AddField("Ship", _dataTradeBuyConfirm.ShipIdentifier, true)
                                .AddField("Total Value", _dataTradeBuyConfirm.Item.TransactionCost.ToString(), false)
                                .AddField($"**{_dataTradeBuyConfirm.Item.Name}** ({_dataTradeBuyConfirm.Item.PricePerUnit} aUEC)", $"{_dataTradeBuyConfirm.Item.Quantity} units", true);
                                await e.Message.RespondAsync(embed: _embed);
                                break;
                            case connectors.azureComputerVision.models.ScreenShotTypes.TradeConsole_SELLConfirm:
                                var _dataTradeSellConfirm = (connectors.azureComputerVision.models.Terminals.Trade.Confirm)_uploadResult.result;

                                _embed = new DiscordEmbedBuilder
                                {
                                    ThumbnailUrl = item.Url,
                                    Color = DiscordColor.Blue,
                                    Title = "Processed Trade Terminal SELL Confirm screen",
                                }
                                .AddField("Member", $"{e.Author.Username}#{e.Author.Discriminator}", true)
                                .AddField($"Server", _guildName, true)
                                .AddField($"Channel", _channelName, true)
                                .AddField("Ship", _dataTradeSellConfirm.ShipIdentifier, true)
                                .AddField("Total Value", _dataTradeSellConfirm.Item.TransactionCost.ToString(), false)
                                .AddField($"**{_dataTradeSellConfirm.Item.Name}** ({_dataTradeSellConfirm.Item.PricePerUnit} aUEC)", $"{_dataTradeSellConfirm.Item.Quantity} units", true);
                                await e.Message.RespondAsync(embed: _embed);
                                break;
                            case connectors.azureComputerVision.models.ScreenShotTypes.FleetManager:
                                break;
                            case connectors.azureComputerVision.models.ScreenShotTypes.RefineryTerminal_SELL:

                                var _dataRefinerySell = (connectors.azureComputerVision.models.Terminals.Refinery.Sell)_uploadResult.result;

                                _embed = new DiscordEmbedBuilder
                                {
                                    ThumbnailUrl = item.Url,
                                    Color = DiscordColor.Yellow,
                                    Title = "Processed Refinery Terminal SELL screen",
                                }
                                .AddField("Member", $"{e.Author.Username}#{e.Author.Discriminator}", true)
                                .AddField($"Server", _guildName, true)
                                .AddField($"Channel", _channelName, true)
                                .AddField("Ship", _dataRefinerySell.ShipIdentifier, true)
                                .AddField("Items available to sell", _dataRefinerySell.Items.Count.ToString(), true)
                                .AddField("Total Value", _dataRefinerySell.TotalValue.ToString(), false);

                                foreach (var availItem in _dataRefinerySell.Items)
                                {
                                    _embed.AddField($"**{availItem.Name}** ({availItem.Amount.ToString()}%)", availItem.Value.ToString(), true);
                                }
                                await e.Message.RespondAsync(embed: _embed);
                                break;
                            case connectors.azureComputerVision.models.ScreenShotTypes.RefineryTerminal_SELLConfirm:
                                var _dataRefineryConfirm = (connectors.azureComputerVision.models.Terminals.Refinery.Confirm)_uploadResult.result;

                                _embed = new DiscordEmbedBuilder
                                {
                                    ThumbnailUrl = item.Url,
                                    Color = DiscordColor.Yellow,
                                    Title = "Processed Refinery Terminal SELL screen",
                                }
                                .AddField("Member", $"{e.Author.Username}#{e.Author.Discriminator}", true)
                                .AddField($"Server", _guildName, true)
                                .AddField($"Channel", _channelName, true)
                                .AddField("Ship", _dataRefineryConfirm.ShipIdentifier, true)
                                .AddField("Total Value", _dataRefineryConfirm.TotalTransactionCost.ToString(), true);

                                await e.Message.RespondAsync(embed: _embed);
                                break;
                            default:
                                break;
                        }


                    }

                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

    }
}

