using EmeraldBot.Model;
using EmeraldBot.Model.Servers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using BlazorStrap;
using EmeraldBot.Model.Characters;

namespace EmeraldBot.Blazor.Pages.Messages
{
    public class AddOrEditBase : ComponentBase
    {
        [Inject] private EmeraldBotContext _ctx { get; set; }
        [Inject] protected IHttpContextAccessor _ca { get; set; }
        [Inject] protected IUriHelper _uri { get; set; }
        [Inject] protected IJSRuntime JSRuntime { get; set; }
        [Parameter] public int MessageID { get; set; }
        [CascadingParameter(Name = "UserID")] protected int UserID { get; set; }
        protected Server Server { get; set; } = null;
        protected PC PC { get; set; } = null;
        protected bool IsEdit => MessageID == -1 ? false : true;
        protected Message Message { get; set; } = new Message();

        protected int TextLength { get; set; } = 0;
        private string _oldIcon;
        private int _oldColour;
        public async Task UpdateCharacterCount() => TextLength = await JSRuntime.InvokeAsync<int>("blazor.getCharacterCount", "MessageText");

        protected override void OnInit()
        {
            try {
                if (UserID == -1) return;

                if (MessageID != -1)
                {
                    Message = _ctx.Messages.SingleOrDefault(x => x.ID == MessageID);
                    if (Message == null) _uri.NavigateTo("messages/");
                    if (Message.Player.ID != UserID) _uri.NavigateTo("messages/");
                    Server = Message.Server;
                    TextLength = Message.Text.Length;
                    _oldIcon = Message.Icon;
                    _oldColour = Message.Colour;
                } else
                {
                    Message.Player = _ctx.Users.Find(UserID);
                    Message.DiscordChannelID = -1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed, probably because of no-authentication: {e.Message}\n{e.StackTrace}");
            }
        }

        public async Task SaveMessage()
        {
            if (string.IsNullOrWhiteSpace(Message.Text)) return;

            if (string.IsNullOrWhiteSpace(Message.Title)) Message.Title = PC.Name;

            //Message.Text = WebUtility.HtmlEncode(Message.Text);
            if (Message.Text.Length >= 2048) return;

            // Send the post
            HubConnection connection = new HubConnectionBuilder()
                    .AddMessagePackProtocol()
                    .WithAutomaticReconnect()
                    .WithUrl(Urls.BotHub,
                    opt =>
                    {
                        opt.Transports = HttpTransportType.WebSockets;
                    })
                    .Build();
            await connection.StartAsync();
            if (!IsEdit)
            {
                Message.Colour = PC.Clan.Colour;
                Message.Icon = PC.Icon;
                Message.Server = Server;
                Message.Player = _ctx.Users.Find(UserID);
                _ctx.Messages.Add(Message);
            }
            else if (PC != null)
            {
                Message.Colour = PC.Clan.Colour;
                Message.Icon = PC.Icon;
            } else
            {
                Message.Colour = _oldColour;
                Message.Icon = _oldIcon;
            }

            _ctx.SaveChanges();

            await connection.InvokeAsync("SendMessage", Message.ID);
            await connection.StopAsync();
            _uri.NavigateTo($"messages/{Message.ID}");
        }

        //protected void PCSelected(PC pc)
        //{
        //    if (pc != null)
        //    {
        //            Message.Icon = pc.Icon;
        //        if (Message.Title == "")
        //            Message.Title = pc.Name;
        //        Message.Colour = (from c in _ctx.Clans join p in _ctx.PCs on c.ID equals p.Clan.ID where p.ID == pc.ID select c).Single().Colour;
        //    } else
        //    {
        //        Message.Icon = _oldIcon;
        //        Message.Colour = _oldColour;
        //    }
        //}
    }
}