using Microsoft.Extensions.Configuration;
using sctm.logging;
using System;
using System.IO;
using System.Reflection;

namespace sctm.discordbot.Commands
{
    public partial class MessageCommands
    {
        private IConfigurationRoot _config;
        private FileLogger _logger;

        public MessageCommands()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
                .Build();

            var _logFilePath = _config["logFilePath"]
                .Replace("{area}", "MessageCommands")
                .Replace(".log", $"{DateTime.Now.ToString("dd-MMM-yyyy")}.log");
            _logger = new FileLogger("discortBot", Environment.MachineName, "Program", _logFilePath);
        }
    }
}
