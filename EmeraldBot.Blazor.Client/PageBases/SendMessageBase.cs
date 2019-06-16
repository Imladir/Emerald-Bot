using EmeraldBot.Blazor.Shared;
using EmeraldBot.Blazor.Shared.Characters;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor.Client.PageBases
{
    public class SendMessageBase : ComponentBase
    {
        [Inject] private HttpClient _client { get; set; }
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
                ChannelID = 578453800470446091,
                CharacterID = 367,
                ServerID = 2,
                Text = this.Text,
                Title = this.Title
            };

            var savedMessage = await _client.PostJsonAsync<Message>(Urls.SendMessage, newMessage);
            Console.WriteLine($"Received message back");
            _uri.NavigateTo($"messages/{savedMessage.ID}");
        }
    }
}
