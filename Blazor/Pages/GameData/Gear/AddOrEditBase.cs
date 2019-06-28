using EmeraldBot.Model;
using EmeraldBot.Model.Game;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor.Pages.GameData.Gear
{
    public class AddOrEditBase : ComponentBase
    {
        [Inject] private EmeraldBotContext _ctx { get; set; }
        [Inject] protected IHttpContextAccessor _ca { get; set; }
        [Inject] protected IUriHelper _uri { get; set; }
        [Inject] protected IJSRuntime JSRuntime { get; set; }
        [CascadingParameter(Name = "UserID")] protected int UserID { get; set; }
        [Parameter] public int GearID { get; set; }
        [Parameter] public string Type { get; set; }
        protected bool IsEdit => GearID == -1 ? false : true;
        protected Model.Game.Gear Item { get; set; }
        protected Weapon Weapon { get { return Item is Weapon ? Item as Weapon : null; } set { Item = value; } }
        protected Armour Armour { get { return Item is Armour ? Item as Armour : null; } set { Item = value; } }
        protected int DescriptionLength { get; set; } = 0;
        public async Task UpdateDescriptionCharacterCount() => DescriptionLength = await JSRuntime.InvokeAsync<int>("blazor.getCharacterCount", "DescriptionText");

        protected override void OnInit()
        {
            try
            {
                if (UserID == -1) return;

                if (GearID != -1)
                {
                    Item = _ctx.Items.SingleOrDefault(x => x.ID == GearID);
                    if (Item == null) _uri.NavigateTo("gear/");

                    // Test that technique is on the right server
                    var canEdit = _ctx.UserRoles.Where(x => x.User.ID == UserID  
                                                         && (x.Role.Name.Equals("Admin")
                                                         || (x.Role.Name.Equals("GM") && x.Server.ID == Item.Server.ID))).SingleOrDefault() != null;
                    if (!canEdit) _uri.NavigateTo("gear/");

                    if (Item is Weapon) Type = "weapon";
                    else if (Item is Armour) Type = "armour";
                    else Type = "general";

                    DescriptionLength = Item.Description.Length;
                } else
                {
                    if (Type.Equals("weapon")) Item = new Weapon();
                    else if (Type.Equals("armour")) Item = new Armour();
                    else Item = new Model.Game.Gear();
                }
                if (Item.Source == null) Item.Source = new Source();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed, probably because of no-authentication: {e.Message}\n{e.StackTrace}");
            }
        }

        protected void Save()
        {
            if (GearID == -1) _ctx.Items.Add(Item);

            if (Type.Equals("weapon"))
            {
                foreach (var g in Weapon.WeaponGrips) g.Weapon = Weapon;
            }
            _ctx.SaveChanges();
            _uri.NavigateTo($"gear/{Item.ID}");
        }
    }
}
