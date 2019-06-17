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
    public class PCSkill
    {
        public int PCID { get; set; }
        public virtual PC PC { get; set; }

        public int SkillID { get; set; }
        public virtual Skill Skill { get; set; }

        [Required]
        public int Value { get; set; }
    }
}
