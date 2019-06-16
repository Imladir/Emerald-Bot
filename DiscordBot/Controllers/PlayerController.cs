using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Controllers
{
    [AllowAnonymous]
    [Route("bot")]
    public class PlayerController : Controller
    {
        [HttpGet]
        [Route("Echo")]
        public IActionResult Echo()
        {
            return Ok("I actually got it to work!");
        }
    }
}
