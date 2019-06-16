using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Game
{
    public class GearQuality
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [RegularExpression(@"^\w{5,16}$", ErrorMessage = "Name is not valid (needs to be a single word between 5 and 16 letters)")]
        public string Name { get; set; }

        public virtual Source Source { get; set; }
        public virtual ICollection<GearQualitiesGear> GearItems { get; set; }
    }
}
