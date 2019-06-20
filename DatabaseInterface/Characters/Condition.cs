using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Characters
{
    [Table("Conditions")]
    public class Condition : NameAlias
    {
        [MaxLength(1024, ErrorMessage = "Description is too long")]
        public string Description { get; set; }

        public string Effect { get; set; }

        public string RemovedWhen { get; set; }

        public virtual ICollection<PCCondition> Characters { get; set; }

        public Condition()
        {
            Description = "";
            Effect = "";
            RemovedWhen = "";
            Characters = new List<PCCondition>();
        }
        public static Condition Get(EmeraldBotContext ctx, string name)
        {
            return ctx.Conditions.Single(x => x.Name.Equals(name));
        }

        public override bool Equals(object obj)
        {
            return obj is Condition condition &&
                   Name.Equals(condition.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }
    }
}
