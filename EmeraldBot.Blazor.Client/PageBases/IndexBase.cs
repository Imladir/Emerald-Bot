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
        protected LoginDetails LoginDetails { get; set; } = new LoginDetails();
        protected bool ShowFailedLogin { get; set; } = false;

        protected override async Task OnInitAsync()
        {
            await LoadServerList();
        }

        private async Task LoadServerList()
        {
            Servers = await _client.GetJsonAsync<List<DiscordServer>>(Urls.ListServers);
        }

        protected async Task Login()
        {
            Console.WriteLine($"Trying to login as {LoginDetails.Username}");
            try
            {
                var success = await _client.PostJsonAsync<bool>(Urls.Login, LoginDetails);
                _uri.NavigateTo("/");
            } catch (HttpRequestException e)
            {
                Console.WriteLine($"Login failed: {e.Message}");
                ShowFailedLogin = true;
            }
        }
    }
}
