using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Net.WebSocket;
using Microsoft.Extensions.Configuration;
using sctm.connectors.azureComputerVision;
using sctm.discordbot.Commands;
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
        static DiscordClient discord;
        private static CommandsNextModule commands;
        private static CommandsNextConfiguration ccfg;
        private static FileLogger logger;

        public static IConfigurationRoot Configuration { get; private set; }

        static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.global.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
                .Build();

            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            var _logAction = "MainAsync";

            var _logFilePath = Configuration["logFilePath"]
                .Replace("{area}", "Program")
                .Replace(".log", $"{DateTime.Now.ToString("dd-MMM-yyyy")}.log");
            logger = new FileLogger("discortBot", Environment.MachineName, "Program", _logFilePath);


            logger.WriteEntry(new logging.Models.LogEntry
            {
                Action = _logAction,
                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                Message = "Configuring discord client"
            });

            var cfg = new DiscordConfiguration
            {
                Token = Configuration["Discord:Token"],
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = false
            };

            discord = new DiscordClient(cfg);

            discord.SetWebSocketClient<WebSocket4NetCoreClient>();

            ccfg = new CommandsNextConfiguration
            {
                // let's use the string prefix defined in config.json
                StringPrefix = Configuration["CommandPrefix"],

                // enable responding in direct messages
                EnableDms = true,

                // enable mentioning the bot as a command prefix
                EnableMentionPrefix = true
            };

            commands = discord.UseCommandsNext(ccfg);

            // let's hook some command events, so we know what's 
            // going on
            commands.CommandExecuted += Commands_CommandExecuted;
            commands.CommandErrored += Commands_CommandErrored;

            // let's add a converter for a custom type and a name

            // up next, let's register our commands
            commands.RegisterCommands<MessageCommands>();

            // set up our custom help formatter
            commands.SetHelpFormatter<SimpleHelpFormatter>();


            #region attachments

            var _attachmentWorker = new AttachmentCommands();

            discord.MessageCreated += async e =>
            {
                if (e.Message.Attachments != null && e.Message.Attachments.Any())
                {
                    foreach (var item in e.Message.Attachments)
                    {
                        if (
                        item.FileName.ToLower().EndsWith(".jpg")
                        || item.FileName.ToLower().EndsWith(".png")
                        )
                            await _attachmentWorker.ProcessImage(item.Id, e);
                    }
                }
            };
            #endregion


            // finally, let's connect and log in
            await discord.ConnectAsync();

            await Task.Delay(-1);
        }

        private static Task Client_Ready(ReadyEventArgs e)
        {
            var _logAction = "Client_Ready";

            logger.WriteEntry(new logging.Models.LogEntry
            {
                Action = _logAction,
                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                Message = "Client is ready to process events."
            });

            return Task.CompletedTask;
        }

        private static Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            var _logAction = "Client_GuildAvailable";

            logger.WriteEntry(new logging.Models.LogEntry
            {
                Action = _logAction,
                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                Message = $"Guild available: {e.Guild.Name}"
            });

            return Task.CompletedTask;
        }

        private static Task Client_ClientError(ClientErrorEventArgs e)
        {
            var _logAction = "Client_ClientError";

            logger.WriteEntry(new logging.Models.LogEntry
            {
                Action = _logAction,
                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                Message = $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}",
                Exception = e.Exception
            });


            return Task.CompletedTask;
        }

        private static Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            var _logAction = "Commands_CommandExecuted";

            logger.WriteEntry(new logging.Models.LogEntry
            {
                Action = _logAction,
                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                Message = $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'"
            });

            return Task.CompletedTask;
        }

        private static async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            var _logAction = "Commands_CommandErrored";

            logger.WriteEntry(new logging.Models.LogEntry
            {
                Action = _logAction,
                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                Message = $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}",
                Exception = e.Exception
            });

            // let's check if the error is a result of lack
            // of required permissions
            if (e.Exception is ChecksFailedException ex)
            {
                // yes, the user lacks required permissions, 
                // let them know

                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                // let's wrap the response into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = new DiscordColor(0xFF0000) // red
                    // there are also some pre-defined colors available
                    // as static members of the DiscordColor struct
                };
                await e.Context.RespondAsync("", embed: embed);
            }

        }
    }
}


