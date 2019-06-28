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

namespace EmeraldBot.Blazor.Shared.Selectors
{
    public class CharacterSelectorBase : ComponentBase
    {
        [Inject] private EmeraldBotContext _ctx { get; set; }
        [CascadingParameter(Name = "UserID")] protected int UserID { get; set; }
        [Parameter] protected Server Server { get; set; }
        [Parameter] protected EventCallback<PC> PCChanged { get; set; }
        [Parameter] protected PC PC { get; set; }
        [Parameter] protected bool AllowEmpty { get; set; } = false;
        protected List<PC> PCs { get; set; } = new List<PC>();
        protected int DefaultCharacter { get; set; }
        private int _lastServerChecked = -2;

        protected override async Task OnParametersSetAsync()
        {
            if (Server == null || Server.ID == _lastServerChecked) return;

            _lastServerChecked = Server.ID;
            await UpdateCharactersList();
        }

        private async Task UpdateCharactersList()
        {
            try
            {
                if (UserID == -1 || Server == null) return;
                PC newCharacter;

                var user = _ctx.Users.Find(UserID);
                // Check if user has privilege and is on owner's channel
                var hasPrivilege = user.Roles.Where(x => x.Server.ID == Server.ID).Any(x => x.Role.Name.Equals("GM")
                                                                                         || x.Role.Name.Equals("Admin"));

                PCs = _ctx.PCs.Where(x => x.Server.ID == Server.ID).OrderBy(x => x.Name).ToList();
                if (!hasPrivilege)
                    PCs = PCs.Where(x => x.Player.ID == UserID && x.Server.ID == Server.ID).OrderBy(x => x.Name).ToList();

                if (PCs.Count > 0 && !AllowEmpty)
                {
                    var df = _ctx.Set<DefaultCharacter>().SingleOrDefault(x => x.Server.ID == Server.ID && x.Player.ID == UserID);
                    if (df == null) newCharacter = PCs[0];
                    else newCharacter = df.Character as PC;
                }
                else newCharacter = null;

                if (newCharacter != PC)
                {
                    PC = newCharacter;
                    await PCChanged.InvokeAsync(PC);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error fetching character selection data {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
