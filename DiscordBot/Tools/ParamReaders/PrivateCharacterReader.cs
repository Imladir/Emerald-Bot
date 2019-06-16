using Discord.Commands;
using EmeraldBot.Model;
using EmeraldBot.Model.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Tools.ParamReaders
{

    public class PrivateCharacterReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            try
            {
                using var ctx = new EmeraldBotContext();
                var pc = new PrivateCharacter() { Character = ctx.GetPlayerCharacter(context.Guild.Id, context.User.Id, input.Replace("$", "")) };
                Console.WriteLine($"I found a matching private character: {pc.Character.Name}");
                return Task.FromResult(TypeReaderResult.FromSuccess(pc));
            }
            catch (Exception)
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Could not find the private character."));
            }
        }
    }
}
