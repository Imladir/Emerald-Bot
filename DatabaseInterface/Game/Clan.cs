using EmeraldBot.Model.Characters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Game
{
    public class Clan
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(64, ErrorMessage = "Name is too long")]
        [Required(ErrorMessage = "Can't create a clan without a name")]
        public string Name { get; set; }

        public string Icon { get; set; }

        public int Colour { get; set; }

        public virtual ICollection<PC> PCs { get; set; }

        public Clan()
        {
            Icon = "https://gamepedia.cursecdn.com/l5r_gamepedia_en/thumb/1/1f/Rings.png/300px-Rings.png";
            Colour = 12354406;
        }
    }
}
