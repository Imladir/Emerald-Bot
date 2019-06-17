using EmeraldBot.Model.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using EmeraldBot.Model;

namespace EmeraldBot.Blazor.Pages.Home
{
    public class IndexBase : ComponentBase
    {
        [Inject] private EmeraldBotContext _ctx { get; set; }
        protected List<Server> Servers { get; set; } = new List<Server>();

        protected override void OnInit()
        {
            Servers = _ctx.Servers.Where(x => x.DiscordID != 0).OrderBy(x => x.Name).ToList();
        }
    }
}
