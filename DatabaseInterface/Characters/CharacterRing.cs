using EmeraldBot.Model.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Characters
{
    public class CharacterRing
    {
        public int CharacterID { get; set; }
        public virtual Character Character { get; set; }

        public int RingID { get; set; }
        public virtual Ring Ring { get; set; }

        [Required]
        public int Value { get; set; }
    }
}
