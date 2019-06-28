using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Identity;
using EmeraldBot.Model.Rolls;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace EmeraldBot.Model.Servers
{
    public class Server
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public long DiscordID { get; set; } = 0;
        [MaxLength()]
        public string Name { get; set; } = "";

        [Required]
        [RegularExpression(@"^[^#@]{1}$", ErrorMessage = "Prefix can't be @ or #")]
        [MaxLength(1)]
        public string Prefix { get; set; } = "!";

        public long DiceChannelID { get; set; } = 0;

        public virtual ICollection<NameAlias> NameAliases { get; set; }
        public virtual ICollection<DefaultCharacter> DefaultCharacters { get; set; }
        public virtual ICollection<Roll> Rolls { get; set; }
        public virtual ICollection<UserRole> Roles { get; set; }

        public bool IsGM(EmeraldBotContext ctx, ulong userID)
        {
            if (ctx.Entry(this).State == EntityState.Detached)
                ctx.Servers.Attach(this);

            var player = ctx.Users.Single(x => x.DiscordID == (long)userID);
            ctx.Entry(this).Collection(x => x.Roles).Load();
            return Roles.Any(x => x.UserID == player.ID && x.Role.Name.Equals("GM"));
        }

        public Character GetCharacter(EmeraldBotContext ctx, string aliasOrName)
        {
            return ctx.Characters.SingleOrDefault(x => x.Server == this 
                                                    && (x.Name.Equals(aliasOrName) || x.Alias.Equals(aliasOrName)));
        }
    }
}
