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
using System.Threading.Tasks;

namespace EmeraldBot.Blazor.Shared.Selectors
{
    public class ChannelSelectorBase : ComponentBase
    {
        [CascadingParameter(Name = "UserID")] protected int UserID { get; set; }
        [Parameter] protected Server Server { get; set; }
        [Parameter] protected EventCallback<long> ChannelIDChanged { get; set; }
        [Parameter] protected long ChannelID { get; set; }
        protected List<DiscordChannel> Channels { get; set; } = new List<DiscordChannel>();
        private int _lastServerChecked = -2;

        protected override async Task OnParametersSetAsync()
        {
            if (Server == null || Server.ID == _lastServerChecked) return;

            _lastServerChecked = Server.ID;
            await UpdateChannels();
        }

        private async Task UpdateChannels()
        {
            try
            {
                if (UserID == -1 || Server == null) return;

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

                Channels.Clear();
                foreach (var c in await connection.InvokeAsync<Dictionary<ulong, string>>("GetUserGuildChannels", UserID, Server.ID))
                    Channels.Add(new DiscordChannel() { ID = (long)c.Key, Name = $"#{c.Value}" });
                await connection.StopAsync();

                Channels = Channels.OrderBy(x => x.Name).ToList();

                if (Channels[0].ID != ChannelID)
                {
                    ChannelID = Channels[0].ID;
                    await ChannelIDChanged.InvokeAsync(ChannelID);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error fetching channel selection data {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
