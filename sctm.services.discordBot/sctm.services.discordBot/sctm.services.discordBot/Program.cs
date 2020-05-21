using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.EventLog;

namespace sctm.services.discordBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile("c:\\dev\\appsettings.global.json", true, true)
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                    .AddEnvironmentVariables();
            })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>()
                      .Configure<EventLogSettings>(config =>
                      {
                          config.SourceName = "ChrispyKoala";
                          config.LogName = "ChrispyKoala-DiscordBot";
                      });
                }).UseWindowsService();
    }
}
