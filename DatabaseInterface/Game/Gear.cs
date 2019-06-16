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
    [Table("Gear")]
    public abstract class Gear : NameAlias
    {
        public virtual Source Source { get; set; }

        [MaxLength(1024, ErrorMessage = "Description is too long")]
        public string Description { get; set; }

        public virtual ICollection<GearQualitiesGear> GearQualities { get; set; }
    }
}
