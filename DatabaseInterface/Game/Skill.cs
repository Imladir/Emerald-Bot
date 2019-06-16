using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Servers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Game
{
    [Table("Skills")]
    public class Skill : NameAlias
    {
        [Required(ErrorMessage = "A skill must be associated to a skill group")]
        public virtual SkillGroup Group { get; set; }

        public virtual Source Source { get; set; }

        public virtual ICollection<TechniqueSkill> Techniques { get; set; }
        public virtual ICollection<CharacterSkill> Characters { get; set; }

        public static Skill Get(EmeraldBotContext ctx, string alias)
        {
            return ctx.Skills.Include(x => x.Group).Single(x => x.Alias.Equals(alias)).LoadServer(ctx) as Skill;
        }
    }
}
