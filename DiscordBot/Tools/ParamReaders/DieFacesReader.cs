using Discord.Commands;
using EmeraldBot.Model;
using EmeraldBot.Model.Rolls;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Tools.ParamReaders
{

    public class DieFacesReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            try
            {
                using var ctx = new EmeraldBotContext();

                List<DieFace> dice = new List<DieFace>();

                // Three different ways to define dice:
                // - as a single string wwrrr for example
                // - as two codes for skill and ring dice: 2s3r or 2s,3r or 2s 3r, etc
                // - as a list of predefined faces: ro,rs,set, etc

                Regex simplePattern = new Regex(@"^((?:[wsrb])+)$", RegexOptions.IgnoreCase);
                Regex complexPattern = new Regex(@"(?<die>(?:\d+[rsbw])|(?:[rswb]\d+))", RegexOptions.IgnoreCase);

                var rollingDice = new Dictionary<char, DieFace>()
                {
                    { 's',  ctx.DieFaces.AsNoTracking().Single(x => x.DieType.Equals("Skill") && x.Value == "")},
                    { 'w',  ctx.DieFaces.AsNoTracking().Single(x => x.DieType.Equals("Skill") && x.Value == "")},
                    { 'r',  ctx.DieFaces.AsNoTracking().Single(x => x.DieType.Equals("Ring") && x.Value == "")},
                    { 'b',  ctx.DieFaces.AsNoTracking().Single(x => x.DieType.Equals("Ring") && x.Value == "")}
                };

                // First, test to see if it matches the simple pattern
                var matches = simplePattern.Matches(input);
                if (matches.Count > 0)
                {
                    foreach (var c in input.ToLower()) dice.Add(rollingDice[c]);
                } else if (complexPattern.Matches(input) is MatchCollection complex && complex.Count > 0)
                {
                    if (complex.Count == 0)
                        return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Could not read dice definitions."));

                    foreach (Match m in complex)
                    {
                        string s = m.Groups[0].Value.ToLower();
                        char type;
                        int n;
                        // Find die type
                        if (s.Contains("w") || s.Contains("s"))
                        {
                            type = 's';
                            n = int.Parse(s.Replace("s", "").Replace("w", ""));
                        } else
                        {
                            type = 'r';
                            n = int.Parse(s.Replace("r", "").Replace("b", ""));
                        }
                        dice.AddRange(Enumerable.Repeat(rollingDice[type], n));
                    }
                } else
                {
                    // We're in the simplest case to parse
                    var diceStr = Regex.Split(input, "[,;]");
                    foreach(var s in diceStr)
                    {
                        var dieType = s[0] == 's' || s[0] == 'w' ? "Skill" : "Ring";
                        string value = s.Substring(1);
                        var reversed = value.Length == 1 ? value : $"{value[1]}{value[0]}";

                        dice.Add(ctx.DieFaces.First(x => x.DieType == dieType
                                                       && (x.Value == value || x.Value == reversed)));
                    }
                }
                return Task.FromResult(TypeReaderResult.FromSuccess(dice.OrderByDescending(x => x.DieType).ToList()));
            }
            catch (Exception)
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Could not parse the command line."));
            }
        }
    }
}
