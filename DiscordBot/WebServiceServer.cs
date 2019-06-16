using Discord.WebSocket;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace EmeraldBot.Bot
{
    public class WebServiceServer
    {
        private static WebServiceServer _instance = null;

        private DiscordSocketClient _discord;
        private readonly IHost _webService;

        public static DiscordSocketClient DiscordClient { get => _instance._discord; }

        public static async Task RunAsync() {
            await _instance._webService.RunAsync();
        }

        private WebServiceServer(string url, DiscordSocketClient discordClient, string[] args)
        {
            try
            {
                _discord = discordClient;

                var config = new ConfigurationBuilder()
                    .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                    .Build();

                // Starting server
                _webService = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(url);
                    webBuilder.UseKestrel();
                    webBuilder.UseIISIntegration();
                    webBuilder.UseStartup<Startup>();
                }).Build();

                Console.WriteLine($"SignalR Server configured at {DateTime.UtcNow:D} on {url}.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error creating webservice: {e.Message}\n{e.StackTrace}");
            }
        }

        public static WebServiceServer Get(string url, DiscordSocketClient discordClient, string[] args)
        {
            if (_instance == null) _instance = new WebServiceServer(url, discordClient, args);

            return _instance;
        }

        public static WebServiceServer Get()
        {
            if (_instance == null) throw new Exception($"Cannot retrieve WebServer: it has not yet been initialised.");

            return _instance;
        }
    }
}
