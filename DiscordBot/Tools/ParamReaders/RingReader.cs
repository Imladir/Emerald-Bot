using Discord.Commands;
using EmeraldBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Tools.ParamReaders
{

    public class RingReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            try
            {
                using var ctx = new EmeraldBotContext();
                var ring = ctx.Rings.Single(x => x.Name.Equals(input));
                return Task.FromResult(TypeReaderResult.FromSuccess(ring));
            }
            catch (Exception)
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Could not parse the ring."));
            }
        }
    }
}
