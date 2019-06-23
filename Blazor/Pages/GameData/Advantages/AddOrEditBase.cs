using EmeraldBot.Model;
using EmeraldBot.Model.Game;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor.Pages.GameData.Advantages
{
    public class AddOrEditBase : ComponentBase
    {
        [Inject] private EmeraldBotContext _ctx { get; set; }
        [Inject] protected IHttpContextAccessor _ca { get; set; }
        [Inject] protected IUriHelper _uri { get; set; }
        [Inject] protected IJSRuntime JSRuntime { get; set; }
        [CascadingParameter(Name = "UserID")] protected int UserID { get; set; }
        [Parameter] public int AdvantageID { get; set; }
        protected int ServerID { get; set; } = -1;
        protected bool IsEdit => AdvantageID == -1 ? false : true;
        protected Advantage Advantage { get; set; } = new Advantage();
        protected int PermanentEffectLength { get; set; } = 0;
        protected int RollEffectLength { get; set; } = 0;
        public async Task UpdatePermanentEffectCharacterCount() => PermanentEffectLength = await JSRuntime.InvokeAsync<int>("blazor.getCharacterCount", "PermanentEffectText");
        public async Task UpdateRollEffectCharacterCount() => RollEffectLength = await JSRuntime.InvokeAsync<int>("blazor.getCharacterCount", "RollEffectText");

        protected override void OnInit()
        {
            try
            {
                if (UserID == -1) return;

                if (AdvantageID != -1)
                {
                    Advantage = _ctx.Advantages.SingleOrDefault(x => x.ID == AdvantageID);
                    if (Advantage == null) _uri.NavigateTo("advantage/");

                    // Test that technique is on the right server
                    ServerID = Advantage.Server.ID;
                    var canEdit = _ctx.UserRoles.Where(x => x.User.ID == UserID  
                                                         && (x.Role.Name.Equals("Admin")
                                                         || (x.Role.Name.Equals("GM") && x.Server.ID == ServerID))).SingleOrDefault() != null;
                    if (!canEdit) _uri.NavigateTo("advantage/");

                    PermanentEffectLength = Advantage.PermanentEffect.Length;
                    RollEffectLength = Advantage.RollEffect.Length;
                }
                if (Advantage.Source == null) Advantage.Source = new Source();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed, probably because of no-authentication: {e.Message}\n{e.StackTrace}");
            }
        }

        protected void Save()
        {
            Advantage.Server = _ctx.Servers.Find(ServerID);
            if (AdvantageID == -1) _ctx.Advantages.Add(Advantage);
            _ctx.SaveChanges();
            _uri.NavigateTo($"advantages/{Advantage.ID}");
        }
    }
}
