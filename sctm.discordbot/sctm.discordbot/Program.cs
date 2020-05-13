using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using sctm.connectors.azureComputerVision;
using sctm.logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
            var _logAction = "MainAsync";

            DirectoryInfo _assemblyLocation = new FileInfo(Assembly.GetCallingAssembly().Location).Directory;
            var _logFilePath = Configuration["logFilePath"].Replace(".log", $"{DateTime.Now.ToString("dd-MMM-yyyy")}.log");
            var _logger = new FileLogger("discortBot", Environment.MachineName, "Program", _logFilePath);


            _client = new HttpClient();
            DiscordEmbedBuilder _embed;

            _logger.WriteEntry(new logging.Models.LogEntry
            {
                Action = _logAction,
                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                Message = "Configuring discord client"
            });

            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = Configuration["Discord:Token"],
                TokenType = TokenType.Bot
            });

            _logger.WriteEntry(new logging.Models.LogEntry
            {
                Action = _logAction,
                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                Message = "Configuring computer vision connector"
            });

            var _uploader = new AzureComputerVisionRepository(new AzureConfiguration
            {
                SubscriptionKey = Configuration["Azure:SubscriptionKey"],
                Endpoint = Configuration["Azure:Endpoint"]
            });

            discord.MessageCreated += async e =>
            {                
                if (e.Message.Content.ToLower().StartsWith("ping"))
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

            

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

    }
}

