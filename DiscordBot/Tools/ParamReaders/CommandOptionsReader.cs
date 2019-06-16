using Discord.Commands;
using EmeraldBot.Model.Characters;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Tools.ParamReaders
{
    public class CommandOptionsReader<T> : TypeReader where T: Character
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            try
            {
                Regex kv_pattern = new Regex("(?<key>\\w+)=(?<paren>[\"“])?(?<value>(?(paren)(.*?)(?=[\"”])|([\\w,;-]+)))[\"”]?", RegexOptions.Singleline);


                var alias = GetAlias(ref input);
                CommandOptions<T> cmdParams = new CommandOptions<T>(context, alias);

                Match m = kv_pattern.Match(input);
                while (m.Success)
                {
                    var key = m.Groups["key"].Value.ToLower().Normalize();
                    var val = m.Groups["value"].Value.Trim().Replace("'", "’").Normalize();
                    if (val.Count() >= 1204)
                        return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"{key} value is too big. Value size limit is 1024."));
                    cmdParams.Params[key] = val;
                    input = input.Replace(m.Value, "");
                    m = m.NextMatch();
                }
                cmdParams.Text = input.Trim();

                return Task.FromResult(TypeReaderResult.FromSuccess(cmdParams));
            }
            catch (Exception)
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Could not parse the command line."));
            }
        }

        private string GetAlias(ref string msg)
        {
            string patternAlias = "\\$(\\S+)";
            string patternName = "\\$\"(.*?)\"";

            Match m = Regex.Match(msg, patternAlias, RegexOptions.IgnoreCase);
            if (!m.Success)
            {
                m = Regex.Match(msg, patternName, RegexOptions.IgnoreCase);
            }

            string alias = "";
            if (m.Success)
            {
                alias = m.Groups[1].Value.ToLower();
                msg = msg.Replace(m.Groups[0].Value, "");
            }

            return alias;
        }
    }
}
