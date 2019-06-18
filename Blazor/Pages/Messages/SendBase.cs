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
        [Inject] private IUriHelper _uri { get; set; }
        [Parameter] protected Character Character { get; set; }
        [Parameter] protected long ChannelID { get; set; }

        protected string Title { get; set; }
        protected string Text { get; set; }

        public async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Text)) return;

            Console.WriteLine($"Title = '{Title}', Text = '{Text}'");
            int serverID = 2;
            ulong channelID = 578453800470446091;
            int userID = 3;
            int characterID = 367;

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

            var msgID = await connection.InvokeAsync<int>("SendPCMessage", serverID, channelID, userID, characterID, Title, Text);
            await connection.StopAsync();
            if (msgID > 0)
            {
                _uri.NavigateTo($"messages/{msgID}");

            } else
            {
                Console.WriteLine("Couldn't send the message for some reason?");
            }

        }
    }
}
