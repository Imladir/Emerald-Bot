using EmeraldBot.Blazor.Shared;
using EmeraldBot.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor.Server.Controllers
{
    public class MessagesController : Controller
    {
        [Inject] private EmeraldBotContext _ctx { get; set; }

        public MessagesController(EmeraldBotContext ctx) { _ctx = ctx; }

        [HttpGet(Urls.Message)]
        public IActionResult GetMessage(int id)
        {
            var msg = _ctx.Messages.SingleOrDefault(x => x.ID == id);

            if (msg == null) return NotFound();

            return Ok(msg.ToBlazor());
        }

        [HttpPost(Urls.SendMessage)]
        public async Task<IActionResult> SendMessage([FromBody] Message msg)
        {
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

            var msgID = await connection.InvokeAsync<int>("SendMessage", msg.Title, msg.Text, msg.ServerID, msg.ChannelID, 1, msg.CharacterID);

            //// Send the new message address
            msg.ID = msgID;
            return Created(new Uri(Urls.Message.Replace("{id}", $"{msgID}"), UriKind.Relative), msg);
        }
    }
}
