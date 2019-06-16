using EmeraldBot.Blazor.Shared;
using EmeraldBot.Blazor.Shared.Characters;
using EmeraldBot.Blazor.Shared.GameData;
using EmeraldBot.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor.Server.Controllers
{
    public class CharactersController : Controller
    {
        private EmeraldBotContext _ctx;

        public CharactersController(EmeraldBotContext dbContext)
        {
            _ctx = dbContext;
        }

        [HttpGet(Urls.GetPC)]
        public IActionResult GetPC(int id) {
            var pc = _ctx.PCs.SingleOrDefault(x => x.ID == id);

            if (pc == null) return NotFound();

            return Ok(pc.ToBlazor());
        }
    }
}
