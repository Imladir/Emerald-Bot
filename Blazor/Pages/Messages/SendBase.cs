using EmeraldBot.Model;
using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Servers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor.Pages.Messages
{
    public class SendBase : ComponentBase
    {
        [Inject] private EmeraldBotContext _ctx { get; set; }
        [Inject] private IUriHelper _uri { get; set; }
        [Parameter] protected Character Character { get; set; }
        [Parameter] protected long ChannelID { get; set; }

        protected string Title { get; set; }
        protected string Text { get; set; }

        public async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Text)) return;

            Console.WriteLine($"Title = '{Title}', Text = '{Text}'");
            var newMessage = new Message()
            {
                DiscordChannelID = 578453800470446091,
                Character = _ctx.PCs.Find(367),
                Server = _ctx.Servers.Find(2),
                Title = Title,
                Text = Text
            };

            // Send the post
            HubConnection connection = new HubConnectionBuilder()
                    .AddMessagePackProtocol()
                    .WithAutomaticReconnect()
                    .WithUrl("http://localhost:5050/emeraldBot",
                    opt =>
                    {
                        opt.Transports = HttpTransportType.WebSockets;
                    })
                    .Build();
            await connection.StartAsync();

            var str = await connection.InvokeAsync<string>("GreetAll");
            var msgID = await connection.InvokeAsync<int>("SendMessage", newMessage);

            _uri.NavigateTo($"messages/{msgID}");
        }
    }
}
