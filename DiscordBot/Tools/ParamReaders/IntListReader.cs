using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Tools.ParamReaders
{

    public class IntListReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            try
            {
                Regex intPattern = new Regex("(\\d+)", RegexOptions.Singleline);

                List<int> res = new List<int>();

                Match m = intPattern.Match(input);
                while (m.Success)
                {
                    res.Add(int.Parse(m.Value) - 1);
                    m = m.NextMatch();
                }

                if (res.Count == 0) throw new Exception($"Could not parse {input} into a list of string.");

                return Task.FromResult(TypeReaderResult.FromSuccess(res));
            }
            catch (Exception)
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Could not parse the command line."));
            }
        }
    }
}
