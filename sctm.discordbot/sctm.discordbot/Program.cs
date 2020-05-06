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
                var _botAvatar = discord.CurrentUser.AvatarUrl;

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
                                await e.Message.RespondAsync(embed: Embeds.RefinerySellConfirm(_dataRefineryConfirm,e,item,_botAvatar));
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

