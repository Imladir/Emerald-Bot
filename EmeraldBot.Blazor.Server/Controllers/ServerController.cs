using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmeraldBot.Blazor.Shared;
using EmeraldBot.Model;
using Microsoft.AspNetCore.Mvc;

namespace EmeraldBot.Blazor.Server.Controllers
{
    public class ServersController : Controller
    {
        private EmeraldBotContext _ctx;

        public ServersController(EmeraldBotContext dbContext)
        {
            _ctx = dbContext;
        }

        [HttpGet(Urls.ListServers)]
        public IActionResult ServerList()
        {
            Console.WriteLine($"Connection string = {EmeraldBotContext.ConnectionString}");
            var servers = _ctx.Servers.Where(x => x.DiscordID != 0);
            var res = new List<DiscordServer>();
            foreach (var s in servers.OrderBy(x => x.Name))
                res.Add(new DiscordServer { ID = s.ID, DiscordID = s.DiscordID, Name = s.Name });

            return Ok(res);
        }

        [HttpGet(Urls.ListPC)]
        public IActionResult List(int id)
        {
            Console.WriteLine($"Trying to get PC list");
            var pcs = _ctx.PCs.Where(x => x.Server.ID == id).Select(x => x.ToBlazor()).OrderBy(x => x.Name).ToList();
            Console.WriteLine($"I found:\n{(string.Join("\n", pcs.Select(x => x.Name)))}");
            return Ok(pcs);
        }
    }
}