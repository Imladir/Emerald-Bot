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
    public class Advantage : NameAlias
    {
        [NotMapped]
        public static readonly List<string> AllowedFields = new List<string>() { "alias", "name", "ring", "rolleffect", "permanenteffect", "type" }.OrderBy(x => x).ToList();
        public virtual AdvantageClass AdvantageClass { get; set; }

        public virtual Ring Ring { get; set; }

        [MaxLength(1024, ErrorMessage = "Permanent effect is too long")]
        public string PermanentEffect { get; set; } = "";

        [MaxLength(1024, ErrorMessage = "Roll effect is too long")]
        public string RollEffect { get; set; } = "";
        public virtual Source Source { get; set; }

        public virtual ICollection<AdvantageTypeAdvantages> AdvantageTypes { get; set; } = new List<AdvantageTypeAdvantages>();
        public virtual ICollection<PCAdvantage> Characters { get; set; }

        public static Advantage Get(EmeraldBotContext ctx, string alias)
        {
            return ctx.Advantages.Include(x => x.AdvantageClass)
                                 .Include(x => x.AdvantageTypes)
                                 .Include(x => x.Ring).Single(x => x.Alias.Equals(alias)).LoadServer(ctx) as Advantage;
        }

        public void Load(EmeraldBotContext ctx)
        {
            ctx.Entry(this).Reference(x => x.AdvantageClass).Load();
            ctx.Entry(this).Reference(x => x.Ring).Load();
            ctx.Entry(this).Reference(x => x.Source).Load();
            ctx.Entry(this).Collection(x => x.AdvantageTypes).Query()
                .Include(x => x.AdvantageType).Load();
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
                    case "type":
                        {
                            AdvantageTypes = new List<AdvantageTypeAdvantages>();
                            Regex.Split(value, "[,;]").ToList().ForEach(x => AdvantageTypes.Add(new AdvantageTypeAdvantages()
                            {
                                Advantage = this,
                                AdvantageType = ctx.AdvantageTypes.Single(y => y.Name.Equals(x))
                            }));
                            return true;
                        }
                    case "ring": Ring = ctx.Rings.Single(x => x.Name.Equals(value)); return true;
                    case "source": var s = value.Split(','); Source = new Source() { Book = s[0], Page = int.TryParse(s[1], out int v) ? v : -1 }; return true;
                    case "permanenteffect": PermanentEffect = value; return true;
                    case "rolleffect": RollEffect = value; return true;
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
