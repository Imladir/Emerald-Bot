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
    public class CharacterSelectorBase : ComponentBase
    {
        [Inject] private IHttpContextAccessor _ca { get; set; }
        [Inject] private EmeraldBotContext _ctx { get; set; }
        protected List<Server> Servers { get; set; } = new List<Server>();
        protected Dictionary<int, PC[]> PCs { get; set; } = new Dictionary<int, PC[]>();
        protected Dictionary<int, List<DiscordChannel>> Channels { get; set; } = new Dictionary<int, List<DiscordChannel>>();
        protected int CurrentServer { get; set; }
        protected ulong CurrentChannel { get; set; }
        protected int CurrentCharacter { get; set; }

        protected override async Task OnInitAsync()
        {
            try
            {
                int.TryParse(_ca.HttpContext.User.Claims.Single(x => x.Type.Equals("UserID")).Value, out int userID);

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

                // Find the servers the user is on
                List<int> ids = await connection.InvokeAsync<List<int>>("GetUserGuilds", userID);
                Servers = ids.Select(x => _ctx.Servers.Find(x)).ToList();
                CurrentServer = Servers[0].ID;

                // Find the characters for the server
                foreach (var s in Servers)
                {
                    Channels[s.ID] = new List<DiscordChannel>();
                    foreach (var c in await connection.InvokeAsync<Dictionary<ulong, string>>("GetUserGuildChannels", userID, s.ID))
                        Channels[s.ID].Add(new DiscordChannel() { ID = c.Key, Name = $"#{c.Value}" });
                    Channels[s.ID] = Channels[s.ID].OrderBy(x => x.Name).ToList();

                    PCs[s.ID] = _ctx.PCs.Where(x => x.Player.ID == userID && x.Server.ID == s.ID).OrderBy(x => x.Name).ToArray();
                }
                await connection.StopAsync();
            } catch (Exception e)
            {
                Console.WriteLine($"Error fetching character selection data {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
