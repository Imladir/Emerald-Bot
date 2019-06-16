using Discord.Commands;
using EmeraldBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Tools.ParamReaders
{

    public class CharacterReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            try
            {
                input = input.Replace("$", "");
                Console.WriteLine("Reading a general character");
                using var ctx = new EmeraldBotContext();
                var c = new GeneralCharacter()
                {
                    Character = ctx.Characters.Single(x => (x.Name.Equals(input) || x.Alias.Equals(input))
                                                        && (x.Server.DiscordID == (long)context.Guild.Id || x.Server.DiscordID == 0))
                };
                Console.WriteLine($"I found a matching general character: {c.Character.Name}");
                return Task.FromResult(TypeReaderResult.FromSuccess(c as GeneralCharacter));
            }
            catch (Exception)
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Could not find the character."));
            }
        }
    }
}
