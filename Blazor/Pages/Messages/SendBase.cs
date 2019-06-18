using EmeraldBot.Model;
using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Servers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
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
        [Inject] private IHttpContextAccessor _ca { get; set; }
        [Inject] private IUriHelper _uri { get; set; }
        protected int CharacterID { get; set; }
        protected int ServerID { get; set; }
        protected ulong ChannelID { get; set; }
        protected Character Character { get; set; }

        protected string Title { get; set; }
        protected string Text { get; set; }

        protected void SelectionChanged(Tuple<int, ulong, int> args)
        {
            ServerID = args.Item1;
            ChannelID = args.Item2;
            CharacterID = args.Item3;
            Character = _ctx.PCs.Find(CharacterID);
        }

        public async Task SendMessage()
        {
            int.TryParse(_ca.HttpContext.User.Claims.Single(x => x.Type.Equals("UserID")).Value, out int userID);

            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Text)) return;

            Console.WriteLine($"Title = '{Title}', Text = '{Text}'");

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

            var msgID = await connection.InvokeAsync<int>("SendPCMessage", ServerID, ChannelID, userID, CharacterID, Title, Text);
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
