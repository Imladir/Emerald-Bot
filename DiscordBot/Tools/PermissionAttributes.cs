﻿using Discord;
using Discord.Commands;
using EmeraldBot.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Tools
{
    class RequireGMAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
            CommandInfo command, IServiceProvider services)
        {
            if (!(context.User is IGuildUser guildUser))
                return Task.FromResult(PreconditionResult.FromError("This command cannot be executed outside of a server."));

            using (var ctx = new EmeraldBotContext())
            {
                bool isGM = ctx.Players.Single(x => x.DiscordID == (long)guildUser.Id).IsGM(guildUser.Guild.Id);

                return isGM
                    ? Task.FromResult(PreconditionResult.FromSuccess())
                    : Task.FromResult(PreconditionResult.FromError("Only the GMs can use this command."));
            }
        }
    }
}
