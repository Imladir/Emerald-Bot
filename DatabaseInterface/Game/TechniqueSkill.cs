using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Game
{
    public class TechniqueSkill
    {
        public int TechniqueID { get; set; }
        public virtual Technique Technique { get; set; }

        public int SkillID { get; set; }
        public virtual Skill Skill{ get; set; }
    }
}
