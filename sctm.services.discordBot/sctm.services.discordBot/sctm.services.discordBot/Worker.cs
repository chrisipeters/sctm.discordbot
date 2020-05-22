using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using sctm.services.discordBot.Commands.Interactive;
using sctm.services.discordBot.Commands.Messages;

namespace sctm.services.discordBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private HttpClient _httpClient;
        private DiscordClient _discord;
        private CommandsNextModule _commands;
        private DiscordDmChannel _supportChannel;

        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Provisioning Services");
            _httpClient = new HttpClient();

            _logger.LogInformation("Configuring Discord client");
            var _dService = new Services(_config, _logger);
            

            var _dBuilder = new DependencyCollectionBuilder();

            _dBuilder.AddInstance<Services>(_dService);
            _dBuilder.AddInstance<ILogger<Worker>>(_logger);
            _dBuilder.AddInstance<IConfiguration>(_config);

            (_discord, _commands) = _dService.CreateDiscordClient(_dBuilder.Build());


            _commands.RegisterCommands<MessageCommands>();
            //_commands.RegisterCommands<InteractiveCommands>();

            _discord.ConnectAsync();

            var _assemblyVersion = typeof(Worker).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

            // setup support channels
            try
            {
                var _supportUserId = _config["Discord:SupportUser"];
                var _supportUser = _discord.GetUserAsync(ulong.Parse(_supportUserId)).Result;

                _supportChannel = _discord.CreateDmAsync(_supportUser).Result;
                _supportChannel.SendMessageAsync($"I'm alive! Now running version {_assemblyVersion}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up support channels");
            }
            
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("ChrispyKoala running at: {time}", DateTimeOffset.Now);
                await Task.Delay(30000, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _supportChannel.SendMessageAsync($"I'm shutting down!");
            _logger.LogInformation("disconnecting from Discord");
            _discord.DisconnectAsync();
            Task.Delay(1000);

            return base.StopAsync(cancellationToken);
        }
    }
}
