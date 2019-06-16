using EmeraldBot.Blazor.Shared;
using EmeraldBot.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor.Server.Controllers
{
    public class LoginController : Controller
    {
        private EmeraldBotContext _ctx;
        public LoginController(EmeraldBotContext dbContext)
        {
            _ctx = dbContext;
        }

        [HttpPost(Urls.Login)]
        public IActionResult Login([FromBody] LoginDetails details)
        {
            if (string.IsNullOrWhiteSpace(details.Username) || string.IsNullOrWhiteSpace(details.Password))
                return BadRequest("Neither username nor password can be empty.");

            var player = _ctx.Players.SingleOrDefault(x => x.Name.Equals(details.Username));
            if (player != null)
            {
                var salt = Convert.FromBase64String(player.Salt);
                string hash = Pbkdf2Hasher.ComputeHash(details.Password, salt);
                var userID = (ulong)player.DiscordID;
                if (player.Password != hash)
                    return BadRequest("Authentication failed. Check login information.");
            }
            else
            {
                return BadRequest("Authentication failed. Check login information.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $"{player.ID}"),
                new Claim("Fullname", player.Name)
            };

            //TODO: Change that with appsettings.json stuff
            if (player.ID == 1)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            if (player.IsGMOn.Count > 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, "GM"));
                claims.Add(new Claim("GmOnServers", string.Join(";", player.IsGMOn.Select(x => x.ServerID))));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IssuedUtc = DateTime.UtcNow
            };


            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                          new ClaimsPrincipal(claimsIdentity),
                                          authProperties);

            return Ok(true);
        }
    }
}
