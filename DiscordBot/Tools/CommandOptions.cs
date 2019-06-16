using Discord.Commands;
using EmeraldBot.Model;
using EmeraldBot.Model.Characters;
using System.Collections.Generic;
using System.Linq;

namespace EmeraldBot.Bot.Tools
{
    public class CommandOptions<T> where T: Character
    {
        public T Target { get; set; }
        public string Text { get; set; }
        public Dictionary<string, string> Params { get; set; }

        public CommandOptions()
        {
            Params = new Dictionary<string, string>();
            Text = "";
        }

        public CommandOptions(ICommandContext cmdContext, string alias = "") : this()
        {
            using var dbContext = new EmeraldBotContext();
            Target = dbContext.GetCommandTarget(cmdContext.Guild.Id, cmdContext.User.Id, alias) as T;
        }
        
        public void Reattach(EmeraldBotContext ctx) { ctx.Characters.Attach(Target); }
    }
}
