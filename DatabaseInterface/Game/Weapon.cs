using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Game
{
    [Table("Weapons")]
    public class Weapon : Gear
    {
        [Required(ErrorMessage = "A weapon must have a type")]
        public virtual WeaponType WeaponType { get; set; }
        public int RangeMin { get; set; }
        public int RangeMax { get; set; }
        public int Damage { get; set; }
        public int Deadliness { get; set; }
        public virtual ICollection<WeaponGripsWeapon> WeaponGrips { get; set; }
    }
}
