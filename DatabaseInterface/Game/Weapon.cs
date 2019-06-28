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
        public int RangeMin { get; set; } = 0;
        public int RangeMax { get; set; } = 0;
        public int Damage { get; set; } = 0;
        public int Deadliness { get; set; } = 0;
        public virtual ICollection<WeaponGripsWeapon> WeaponGrips { get; set; }
    }
}
