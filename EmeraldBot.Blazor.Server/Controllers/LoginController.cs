using EmeraldBot.Blazor.Shared;
using EmeraldBot.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor.Server.Controllers
{
    public class LoginController : Controller
    {
        private IConfiguration _config;
        private EmeraldBotContext _ctx;
        public LoginController(IConfiguration config, EmeraldBotContext dbContext)
        {
            _config = config;
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


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(Convert.ToInt32(_config["JwtExpiryInDays"]));
            var token = new JwtSecurityToken(
                _config["JwtIssuer"],
                _config["JwtIssuer"],
                claims,
                expires: expiry,
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });

            //var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //var authProperties = new AuthenticationProperties
            //{
            //    AllowRefresh = true,
            //    IssuedUtc = DateTime.UtcNow
            //};


            //HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            //                              new ClaimsPrincipal(claimsIdentity),
            //                              authProperties);

            //return Ok(true);
        }
    }
}
