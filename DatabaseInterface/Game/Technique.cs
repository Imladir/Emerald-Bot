using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Servers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Game
{
    [Table("Techniques")]
    public class Technique : NameAlias
    {
        [NotMapped]
        public static readonly List<string> AllowedFields = new List<string>() { "activation", "alias", "effect", "name", "rank", "ring", "skill", "source", "type", "tn" }.OrderBy(x => x).ToList();
        public virtual TechniqueType Type { get; set; }
        public int Rank { get; set; }
        public virtual Ring Ring { get; set; }
        public int TN { get; set; }
        public virtual Source Source { get; set; }
        public string Activation { get; set; }
        public string Effect { get; set; }

        public virtual ICollection<TechniqueSkillGroup> SkillGroups { get; set; }
        public virtual ICollection<TechniqueSkill> Skills { get; set; }
        public virtual ICollection<PCTechnique> PCs { get; set; }

        public static Technique Get(EmeraldBotContext ctx, string alias)
        {
            return ctx.Techniques.Include(x => x.Type)
                                 .Include(x => x.Ring).Single(x => x.Alias.Equals(alias)).LoadServer(ctx) as Technique;
        }

        public override void FullLoad(EmeraldBotContext ctx)
        {
            base.FullLoad(ctx);
            ctx.Entry(this).Reference(x => x.Ring).Load();
            ctx.Entry(this).Collection(x => x.Skills).Load();
            ctx.Entry(this).Collection(x => x.SkillGroups).Load();
        }

        public override void Update(EmeraldBotContext ctx, Dictionary<string, string> args)
        {
            foreach (var kv in args)
                if (!base.UpdateField(ctx, kv.Key, kv.Value)) UpdateField(ctx, kv.Key, kv.Value);
        }

        public override bool UpdateField(EmeraldBotContext ctx, string field, string value)
        {
            try
            {
                switch (field.ToLower())
                {
                    case "type": Type = ctx.TechniqueTypes.Single(x => x.Name.Equals(value)); return true;
                    case "rank": Rank = int.Parse(value); return true;
                    case "ring": Ring = ctx.Rings.Single(x => x.Name.Equals(value)); return true;
                    case "tn": TN = int.Parse(value); return true;
                    case "source": var s = value.Split(','); Source = new Source() { Book = s[0], Page = int.TryParse(s[1], out int v) ? v : -1 }; return true;
                    case "activation": Activation = value; return true;
                    case "effect": Effect = value; return true;
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine($"Caught an exception on update for {field} with value {value}:\n{e.Message}\n{e.StackTrace}");
                throw new Exception($"Couldn't update {field} with value {value}: {e.Message}");
            }
            return false;
        }
    }
}
