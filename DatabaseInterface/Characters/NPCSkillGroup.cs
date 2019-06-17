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
    public class NPCSkillGroup
    {
        public int NPCID { get; set; }
        public virtual NPC NPC { get; set; }
        public int SkillGroupID { get; set; }

        public virtual SkillGroup SkillGroup { get; set; }

        [Required]
        public int Value { get; set; }
    }
}
