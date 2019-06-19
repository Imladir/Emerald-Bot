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

namespace EmeraldBot.Blazor.Shared
{
    public class ChannelSelectorBase : ComponentBase
    {
        [CascadingParameter(Name = "UserID")] protected int UserID { get; set; }
        [Parameter] protected EventCallback<ulong> OnSelected { get; set; }
        [Parameter] protected int ServerID { get; set; }
        private int lastKnownServer;
        protected List<DiscordChannel> Channels { get; set; } = new List<DiscordChannel>();
        protected ulong CurrentChannel { get; set; } = 0;

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                if (UserID == -1 || lastKnownServer == ServerID) return;
                lastKnownServer = ServerID;
                var forceUpdate = CurrentChannel == 0;

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
                foreach (var c in await connection.InvokeAsync<Dictionary<ulong, string>>("GetUserGuildChannels", UserID, ServerID))
                        Channels.Add(new DiscordChannel() { ID = c.Key, Name = $"#{c.Value}" });
                    Channels = Channels.OrderBy(x => x.Name).ToList();

                if (Channels[0].ID != CurrentChannel)
                {
                    CurrentChannel = Channels[0].ID;
                    await connection.StopAsync();
                    if (forceUpdate) await OnSelected.InvokeAsync(CurrentChannel);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error fetching channel selection data {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
