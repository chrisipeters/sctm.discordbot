using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Net.WebSocket;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace sctm.services.discordBot
{
    public partial class Services
    {
        public (DiscordClient client, CommandsNextModule cNext) CreateDiscordClient(DependencyCollection dependencies)
        {
            _discord = new DiscordClient(_cfg);

            _discord.SetWebSocketClient<WebSocket4NetCoreClient>();

            _ccfg = new CommandsNextConfiguration
            {
                // let's use the string prefix defined in config.json
                StringPrefix = _config["CommandPrefix"],

                // enable responding in direct messages
                EnableDms = true,

                // enable mentioning the bot as a command prefix
                EnableMentionPrefix = true,
                Dependencies = dependencies
            };

            

            _commands = _discord.UseCommandsNext(_ccfg);

            _commands.CommandExecuted += Commands_CommandExecuted;
            _commands.CommandErrored += Commands_CommandErrored;

            return (_discord, _commands);
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            _logger.LogInformation("Client is ready to process events");

            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            _logger.LogInformation($"Guild available: {e.Guild.Name}");

            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            _logger.LogError(e.Exception, $"Client: {e.Client.CurrentApplication.Name} experienced an exception error");

            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            _logger.LogInformation($"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'");

            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            _logger.LogError(e.Exception, $"Command: {e.Command.Name}  experienced an exception error");

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
