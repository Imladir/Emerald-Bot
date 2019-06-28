using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Game
{
    public class WeaponGrip
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int Hands { get; set; } = 1;

        public int NewRangeMin { get; set; } = -1;
        public int NewRangeMax { get; set; } = -1;
        public int DamageModificator { get; set; } = 0;
        public int DeadlinessModificator { get; set; } = 0;
        public virtual ICollection<WeaponGripsWeapon> Weapons { get; set; }

        public WeaponGrip(int hands)
        {
            Hands = hands;
        }
    }
}
