using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Game
{
    [Table("Armours")]
    public class Armour : Gear
    {
        [Required]
        public int Physical { get; set; } = 0;
        [Required]
        public int Spiritual { get; set; } = 0;
    }
}
