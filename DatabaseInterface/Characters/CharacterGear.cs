using EmeraldBot.Model.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Characters
{
    public class CharacterGear
    {
        public int CharacterID { get; set; }
        public virtual Character Character { get; set; }
        public int GearID { get; set; }
        public virtual Gear Gear { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
