using Microsoft.Extensions.Configuration;
using sctm.logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace sctm.discordbot.Commands
{
    public partial class AttachmentCommands
    {
        private IConfigurationRoot _config;
        private FileLogger _logger;
        private HttpClient _httpClient;
        private Services _services;

        public AttachmentCommands()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.global.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
                .Build();

            var _logFilePath = _config["logFilePath"]
                .Replace("{area}", "AttachmentCommands")
                .Replace(".log", $"{DateTime.Now.ToString("dd-MMM-yyyy")}.log");
            _logger = new FileLogger("discortBot", Environment.MachineName, "Program", _logFilePath);

            _httpClient = new HttpClient();

            _services = new Services(_config, _httpClient, _logger);
        }
    }
}
