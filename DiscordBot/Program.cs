using Discord;
using Discord.Commands;
using Discord.WebSocket;
using EmeraldBot.Bot.Tools;
using EmeraldBot.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EmeraldBot.Bot
{
    class Program
    {

        CommandHandler _handler;
        WebServiceServer _webService;


        static void Main(string[] args)
        => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            var token = Environment.GetEnvironmentVariable("bot_token");
            if (token == null || token == "")
            {
                Console.WriteLine($"Token is absent or invalid. Please check configuration.");
                Console.ReadLine();
                return;
            }

            /** Set the DB Context connection string **/
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            //EmeraldBotContext.ConnectionString = config.GetSection("ConnectionStrings")["azure"];

            /* Discord */
            var client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            client.Log += Log;
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            var commands = new CommandService();
            var services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .BuildServiceProvider();

            _handler = new CommandHandler(commands, client, services);
            //await _handler.SetupAsync(client);

            string url = "http://localhost:5050/";
            _webService = WebServiceServer.Get(url, client, args);

            var _ = WebServiceServer.RunAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            var c = Console.ForegroundColor;

            switch (msg.Severity)
            {
                case LogSeverity.Critical: Console.ForegroundColor = ConsoleColor.DarkRed; break;
                case LogSeverity.Error: Console.ForegroundColor = ConsoleColor.Red; break;
                case LogSeverity.Warning: Console.ForegroundColor = ConsoleColor.Yellow; break;
                case LogSeverity.Info: Console.ForegroundColor = ConsoleColor.White; break;
                case LogSeverity.Verbose: Console.ForegroundColor = c; break;
                case LogSeverity.Debug: Console.ForegroundColor = c; break;
            }
            Console.WriteLine(String.Format("{0:yy-MM-dd HH:mm:ss} [{1,8}] {2,10}: {3}", DateTime.Now, msg.Severity, msg.Source, msg.Message));

            Console.ForegroundColor = c;
            return Task.CompletedTask;
        }
    }
}
