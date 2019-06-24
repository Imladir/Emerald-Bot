using EmeraldBot.Model;
using EmeraldBot.Model.Characters;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor.Pages.Characters
{
    public class AddOrEditBase : ComponentBase
    {
        [Inject] private EmeraldBotContext _ctx { get; set; }
        [Inject] protected IHttpContextAccessor _ca { get; set; }
        [Inject] protected IUriHelper _uri { get; set; }
        [Inject] protected IJSRuntime JSRuntime { get; set; }
        [CascadingParameter(Name = "UserID")] protected int UserID { get; set; }
        [Parameter] public int CharacterID { get; set; }
        protected bool IsEdit => CharacterID == -1 ? false : true;
        protected PC PC { get; set; } = new PC();
        protected int DescriptiontLength { get; set; } = 0;
        public async Task UpdateDescriptiontCharacterCount() => DescriptiontLength = await JSRuntime.InvokeAsync<int>("blazor.getCharacterCount", "DescriptionText");

        protected override void OnInit()
        {
            try
            {
                if (UserID == -1 || PC.Description.Length > 1024) return;

                if (CharacterID != -1)
                {
                    PC = _ctx.PCs.SingleOrDefault(x => x.ID == CharacterID);
                    if (PC == null) _uri.NavigateTo("characters/");

                    // Test that technique is on the right server
                    var canEdit = _ctx.UserRoles.Where(x => x.User.ID == UserID  
                                                         && (x.Role.Name.Equals("Admin")
                                                         || (x.Role.Name.Equals("GM") && x.Server.ID == PC.Server.ID))).SingleOrDefault() != null;
                    if (!canEdit) _uri.NavigateTo($"characters/{CharacterID}");

                    DescriptiontLength = PC.Description.Length;
                } else
                {
                    PC.Player = _ctx.Users.Find(UserID);
                    PC.InitRings(_ctx);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed, probably because of no-authentication: {e.Message}\n{e.StackTrace}");
            }
        }

        protected void Save()
        {
            if (CharacterID == -1) _ctx.PCs.Add(PC);
            _ctx.SaveChanges();
            _uri.NavigateTo($"characters/{PC.ID}");
        }
    }
}
