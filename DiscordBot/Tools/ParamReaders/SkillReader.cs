using Discord.Commands;
using EmeraldBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Tools.ParamReaders
{

    public class SkillReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            try
            {
                using var ctx = new EmeraldBotContext();
                var skill = ctx.Skills.Single(x => (x.Name.Equals(input) || x.Alias.Equals(input))
                                                && (x.Server.DiscordID == (long)context.Guild.Id || x.Server.DiscordID == 0));
                return Task.FromResult(TypeReaderResult.FromSuccess(skill));
            }
            catch (Exception)
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Could not parse the skill."));
            }
        }
    }
}
