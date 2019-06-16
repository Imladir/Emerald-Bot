using EmeraldBot.Model.Servers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model
{
    [Table("NameAliases")]
    public abstract class NameAlias
    {
        [NotMapped]
        public static readonly List<string> AcceptedFields = new List<string>() { "name", "alias" }.OrderBy(s => s).ToList();

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [RegularExpression(@"^\w{3,24}$", ErrorMessage = "Alias is not valid (needs to be a single word between 3 and 24 letters)")]
        public string Alias { get; set; }

        [Required]
        [MinLength(3), MaxLength(128)]
        public string Name { get; set; }
        [Required]
        public virtual Server Server { get; set; }

        public virtual void Update(EmeraldBotContext ctx, Dictionary<string, string> args)
        {
            foreach (var kv in args) UpdateField(ctx, kv.Key, kv.Value);
        }

        public virtual bool UpdateField(EmeraldBotContext ctx, string field, string value)
        {
            switch (field.ToLower())
            {
                case "name": Name = value; return true;
                case "alias": Alias = value; return true;
            }
            return false;
        }

        public virtual void FullLoad(EmeraldBotContext ctx)
        {
            ctx.Entry(this).Reference(x => x.Server).Load();
        }

        public NameAlias LoadServer(EmeraldBotContext ctx)
        {
            ctx.NameAliases.Attach(this);
            ctx.Entry(this).Reference(x => x.Server).Load();
            return this;
        }
    }
}
