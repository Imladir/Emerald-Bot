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

namespace EmeraldBot.Blazor.Shared
{
    public class CharacterSelectorBase : ComponentBase
    {
        [Inject] private EmeraldBotContext _ctx { get; set; }
        [CascadingParameter(Name = "UserID")] protected int UserID { get; set; }
        [Parameter] protected EventCallback<int> OnSelected { get; set; }
        [Parameter] protected int ServerID { get; set; }
        [Parameter] protected bool AllowEmpty { get; set; } = false;
        protected List<PC> PCs { get; set; } = new List<PC>();
        protected int CurrentCharacter { get; set; }
        protected int DefaultCharacter { get; set; }
        private int lastKnownServer { get; set; } = 0;

        protected override void OnParametersSet()
        {
            UpdateCharactersList();
        }

        private void UpdateCharactersList()
        {
            try
            {
                if (UserID == -1 || lastKnownServer == ServerID) return;
                lastKnownServer = ServerID;
                var needUpdate = CurrentCharacter == 0;

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
                    if (df == null) CurrentCharacter = PCs[0].ID;
                    else CurrentCharacter = df.Character.ID;
                }
                else CurrentCharacter = -1;
                if (needUpdate) OnSelected.InvokeAsync(CurrentCharacter);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error fetching character selection data {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
