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
        public int Hands { get; set; }

        public int NewRangeMin { get; set; }
        public int NewRangeMax { get; set; }
        public int DamageModificator { get; set; }
        public int DeadlinessModificator { get; set; }
        public virtual ICollection<WeaponGripsWeapon> Weapons { get; set; }

        public WeaponGrip(int hands)
        {
            Hands = hands;
            NewRangeMin = -1;
            NewRangeMax = -1;
            DamageModificator = 0;
            DeadlinessModificator = 0;
        }
    }
}
