using EmeraldBot.Model.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Characters
{
    public class PCAdvantage
    {
        public int CharacterID { get; set; }
        public virtual Character Character { get; set; }

        public int AdvantageID { get; set; }
        public virtual Advantage Advantage { get; set; }
    }
}
