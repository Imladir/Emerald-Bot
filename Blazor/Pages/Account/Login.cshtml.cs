using EmeraldBot.Model;
using EmeraldBot.Model.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor.Pages.Account
{
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        private readonly EmeraldBotContext _ctx;
        private readonly SignInManager<User> _signInManager;
        /*private readonly ILogger<LoginModel> _logger;*/

        public LoginModel(EmeraldBotContext ctx, SignInManager<User> signInManager/*, ILogger<LoginModel> logger*/)
        {
            _ctx = ctx;
            _signInManager = signInManager;
            /*_logger = logger;*/
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // Get User
                var user = _ctx.Users.SingleOrDefault(x => x.UserName.Equals(Username));
                if (user != null)
                {
                    // Check password
                    var passMatch = await _signInManager.CheckPasswordSignInAsync(user, Password, false);
                    if (passMatch.Succeeded)
                    {
                        var principal = _signInManager.CreateUserPrincipalAsync(user);
                        await _signInManager.SignInAsync(user, RememberMe);
                        return LocalRedirect(returnUrl);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}