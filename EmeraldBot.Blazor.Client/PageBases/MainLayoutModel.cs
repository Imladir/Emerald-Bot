using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor.Client.PageBases
{
    public class MainLayoutModel : LayoutComponentBase
    {
        [Inject] protected UserAuthentication AppState { get; set; }

        protected async Task Logout()
        {
            await AppState.Logout();
        }
    }
}