using EmeraldBot.Blazor.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor.Client
{
    public class LoginBase : ComponentBase
    {
        [Inject] private HttpClient _client { get; set; }
        [Inject] private IUriHelper _uri { get; set; }
        protected LoginDetails LoginDetails { get; set; } = new LoginDetails();
        protected bool ShowFailedLogin { get; set; } = false;

        protected async Task Login()
        {
            Console.WriteLine($"Trying to login as {LoginDetails.Username}");
            try
            {
                var success = await _client.PostJsonAsync<bool>(Urls.Login, LoginDetails);
                _uri.NavigateTo("/");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Login failed: {e.Message}");
                ShowFailedLogin = true;
            }
        }
    }
}
