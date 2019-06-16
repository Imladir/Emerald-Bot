using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Game
{
    public class TechniqueSkillGroup
    {
        public int TechniqueID { get; set; }
        public virtual Technique Technique { get; set; }

        public int SkillGroupID { get; set; }
        public virtual SkillGroup SkillGroup { get; set; }
    }
}
