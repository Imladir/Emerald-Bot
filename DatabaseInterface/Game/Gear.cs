using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Servers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Game
{
    public class Gear : NameAlias
    {
        public virtual Source Source { get; set; }
        public string Description { get; set; } = "";
        public virtual GearRarity Rarity { get; set; } = new GearRarity();
        public virtual MoneySum Cost { get; set; } = new MoneySum();

        public virtual ICollection<GearQualitiesGear> GearQualities { get; set; } = new List<GearQualitiesGear>();
        public virtual ICollection<CharacterGear> Characters { get; set; }
    }
}
