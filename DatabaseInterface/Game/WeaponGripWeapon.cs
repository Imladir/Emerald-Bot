using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Game
{
    public class WeaponGripsWeapon
    {
        public int WeaponID { get; set; }
        public virtual Weapon Weapon { get; set; }

        public int WeaponGripID { get; set; }
        public virtual WeaponGrip WeaponGrip { get; set; }
    }
}
