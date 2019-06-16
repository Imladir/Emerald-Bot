using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EmeraldBot.Blazor.Shared;
using EmeraldBot.Blazor.Shared.Characters;
using Microsoft.AspNetCore.Components;

namespace EmeraldBot.Blazor.Client
{
    public class PCListBase : ComponentBase
    {
        [Inject] private HttpClient _client { get; set; }
        [Parameter] public int ServerID { get; set; }
        protected List<PC> PCs { get; set; } = new List<PC>();


        protected override async Task OnParametersSetAsync()
        {
            await LoadPCsList();
        }

        private async Task LoadPCsList()
        {
            if (ServerID == 1) return;
            Console.WriteLine($"Trying to reach {Urls.ListPC.Replace("{id}", $"{ServerID}")}");
            PCs = await _client.GetJsonAsync<List<PC>>(Urls.ListPC.Replace("{id}", $"{ServerID}"));
            Console.WriteLine($"PCs = {PCs}");
            Console.WriteLine($"Got the result for {ServerID}, and found {PCs.Count} characters");
        }
    }
}
