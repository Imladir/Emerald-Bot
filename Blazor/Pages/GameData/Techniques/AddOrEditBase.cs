using EmeraldBot.Model;
using EmeraldBot.Model.Game;
using EmeraldBot.Model.Servers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor.Pages.GameData.Techniques
{
    public class AddOrEditBase : ComponentBase
    {
        [Inject] private EmeraldBotContext _ctx { get; set; }
        [Inject] protected IHttpContextAccessor _ca { get; set; }
        [Inject] protected IUriHelper _uri { get; set; }
        [Inject] protected IJSRuntime JSRuntime { get; set; }
        [CascadingParameter(Name = "UserID")] protected int UserID { get; set; }
        [Parameter] public int TechniqueID { get; set; }
        protected bool IsEdit => TechniqueID == -1 ? false : true;
        protected Technique Technique { get; set; } = new Technique();
        protected int ActivationLength { get; set; } = 0;
        protected int EffectLength { get; set; } = 0;
        public async Task UpdateActivationCharacterCount() => ActivationLength = await JSRuntime.InvokeAsync<int>("blazor.getCharacterCount", "TechniqueActivationText");
        public async Task UpdateEffectCharacterCount() => EffectLength = await JSRuntime.InvokeAsync<int>("blazor.getCharacterCount", "TechniqueEffectText");

        protected override void OnInit()
        {
            try
            {
                if (UserID == -1) return;

                if (TechniqueID != -1)
                {
                    Technique = _ctx.Techniques.SingleOrDefault(x => x.ID == TechniqueID);
                    if (Technique == null) _uri.NavigateTo("techniques/");

                    // Test that technique is on the right server
                    var canEdit = _ctx.UserRoles.Where(x => x.User.ID == UserID  
                                                         && (x.Role.Name.Equals("Admin")
                                                         || (x.Role.Name.Equals("GM") && x.Server.ID == Technique.Server.ID))).SingleOrDefault() != null;
                    if (!canEdit) _uri.NavigateTo("techniques/");

                    ActivationLength = Technique.Activation.Length;
                    EffectLength = Technique.Effect.Length;
                }
                if (Technique.Source == null) Technique.Source = new Source();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed, probably because of no-authentication: {e.Message}\n{e.StackTrace}");
            }
        }

        protected void Save()
        {
            if (TechniqueID == -1) _ctx.Techniques.Add(Technique);
            _ctx.SaveChanges();
            _uri.NavigateTo($"techniques/{Technique.ID}");
        }
    }
}
