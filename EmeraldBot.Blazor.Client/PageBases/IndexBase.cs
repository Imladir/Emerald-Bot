using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using EmeraldBot.Blazor.Shared;
using Microsoft.AspNetCore.Components;

namespace EmeraldBot.Blazor.Client
{
    public class IndexBase : ComponentBase
    {
        [Inject] private HttpClient _client { get; set; }
        [Inject] private IUriHelper _uri { get; set; }
        protected List<DiscordServer> Servers { get; set; } = new List<DiscordServer>();

        protected override async Task OnInitAsync()
        {
            await LoadServerList();
        }

        private async Task LoadServerList()
        {
            Servers = await _client.GetJsonAsync<List<DiscordServer>>(Urls.ListServers);
        }
    }
}
