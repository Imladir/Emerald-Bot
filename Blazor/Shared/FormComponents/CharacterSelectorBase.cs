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

namespace EmeraldBot.Blazor.Shared.FormComponents
{
    public class CharacterSelectorBase : ComponentBase
    {
        [Inject] private EmeraldBotContext _ctx { get; set; }
        [CascadingParameter(Name = "UserID")] protected int UserID { get; set; }
        [Parameter] protected int ServerID { get; set; }
        [Parameter] protected EventCallback<int> CharacterIDChanged { get; set; }
        [Parameter] protected int CharacterID { get; set; }
        [Parameter] protected bool AllowEmpty { get; set; } = false;
        protected List<PC> PCs { get; set; } = new List<PC>();
        protected int DefaultCharacter { get; set; }
        private int _lastServerChecked = -2;

        protected override async Task OnParametersSetAsync()
        {
            if (ServerID == _lastServerChecked) return;

            _lastServerChecked = ServerID;
            await UpdateCharactersList();
        }

        private async Task UpdateCharactersList()
        {
            try
            {
                if (UserID == -1 || ServerID == -1) return;
                int newCharacterID;

                var user = _ctx.Users.Find(UserID);
                // Check if user has privilege and is on owner's channel
                var hasPrivilege = user.Roles.Where(x => x.Server.ID == ServerID).Any(x => x.Role.Name.Equals("GM")
                                                                                        || x.Role.Name.Equals("Admin"));

                PCs = _ctx.PCs.Where(x => x.Server.ID == ServerID).OrderBy(x => x.Name).ToList();
                if (!hasPrivilege)
                    PCs = PCs.Where(x => x.Player.ID == UserID && x.Server.ID == ServerID).OrderBy(x => x.Name).ToList();

                if (PCs.Count > 0 && !AllowEmpty)
                {
                    var df = _ctx.Set<DefaultCharacter>().SingleOrDefault(x => x.Server.ID == ServerID && x.Player.ID == UserID);
                    if (df == null) newCharacterID = PCs[0].ID;
                    else newCharacterID = df.Character.ID;
                }
                else newCharacterID = -1;

                if (newCharacterID != CharacterID)
                {
                    CharacterID = newCharacterID;
                    await CharacterIDChanged.InvokeAsync(CharacterID);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error fetching character selection data {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
