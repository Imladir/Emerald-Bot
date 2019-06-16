using Discord.Commands;
using EmeraldBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Tools.ParamReaders
{

    public class NameAliasReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            try
            {
                using var ctx = new EmeraldBotContext();
                var nameAlias = ctx.GetNameAliasEntity(context.Guild.Id, input);
                return Task.FromResult(TypeReaderResult.FromSuccess(nameAlias));
            }
            catch (Exception)
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Nothing with this name or alias exists."));
            }
        }
    }
}
