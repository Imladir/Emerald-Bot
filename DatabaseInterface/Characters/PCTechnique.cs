using EmeraldBot.Model.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Characters
{
    public class PCTechnique
    {
        public int CharacterID { get; set; }
        public virtual Character Character { get; set; }

        public int TechniqueID { get; set; }
        public virtual Technique Technique { get; set; }
    }
}
