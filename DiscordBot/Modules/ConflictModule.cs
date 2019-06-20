using Discord;
using Discord.Commands;
using EmeraldBot.Bot.Tools;
using EmeraldBot.Model;
using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Modules
{
    [Group("conflict")]
    public class Conflicts : ModuleBase<SocketCommandContext>
    {
        [RequireGM(Group = "Permission")]
        [Command("new")]
        [Summary("Initialises a new comflict")]
        public async Task NewConflict([Summary("Conflict type, one of Duel, Intrigue, MassBattle or Skirmish (Skirmish is the default)")] string type = "skirmish")
        {
            try
            {
                using var ctx = new EmeraldBotContext();
                var conflict = new Conflict()
                {
                    ConflictType = ctx.ConflictTypes.SingleOrDefault(x => x.Name.Equals(type)),
                    Server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id)
                };
                ctx.Conflicts.Add(conflict);
                ctx.SaveChanges();
                await ReplyAsync("", false, SimplePrint("A new conflict has been initialised."));
                await Context.Channel.DeleteMessageAsync(Context.Message);
            } catch (Exception e)
            {
                await ReplyAsync($"Could not start a new conflict: {e.Message}");
                Console.WriteLine($"Could not start a new conflict: {e.Message}\n{e.StackTrace}");
            }
        }

        [Command("join")]
        [Summary("Joins a conflict")]
        public async Task JoinConflict([Summary("Initiative score")] int initiative,
                                       [Summary("Name or alias of the character (or NPC type) joining the conflict.")] CommandOptions<PC> options = null)
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                options ??= new CommandOptions<PC>(Context);
                var conflict = ctx.Conflicts.Where(x => x.Server.DiscordID == (long)Context.Guild.Id).OrderByDescending(x => x.ID).FirstOrDefault();
                if (conflict == null) throw new Exception($"There are no active conflicts. Create one first.");

                ctx.Entry(options.Target).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                conflict.AddParticipant(options.Target, initiative);
                ctx.SaveChanges();

                await ReplyAsync("", false, SimplePrint($"{options.Target.Name} has been added successfully to the conflict."));
                await Context.Channel.DeleteMessageAsync(Context.Message);
            } catch (Exception e)
            {
                await ReplyAsync($"Could not join the conflict: {e.Message}");
                Console.WriteLine($"Could not join the conflict: {e.Message}\n{e.StackTrace}");
            }
        }

        [Command("add")]
        [Summary("Adds a random / no name NPC to the conflict")]
        public async Task AddNoName([Summary("Initiative score")] int initiative,
                                       [Summary("Display name of the NPC joining the conflict")] string name = "")
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                var conflict = ctx.Conflicts.Where(x => x.Server.DiscordID == (long)Context.Guild.Id).OrderByDescending(x => x.ID).FirstOrDefault();
                if (conflict == null) throw new Exception($"There are no active conflicts. Create one first.");

                name = conflict.AddParticipant(name, initiative);
                ctx.SaveChanges();

                await ReplyAsync("", false, SimplePrint($"{name} has been added successfully to the conflict."));
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Could not join the conflict: {e.Message}");
                Console.WriteLine($"Could not join the conflict: {e.Message}\n{e.StackTrace}");
            }
        }

        [Command("next")]
        [Summary("Moves the conflict along to the next participant")]
        public async Task Next()
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                var conflict = ctx.Conflicts.Where(x => x.Server.DiscordID == (long)Context.Guild.Id).OrderByDescending(x => x.ID).FirstOrDefault();
                if (conflict == null) throw new Exception($"There are no active conflicts. Create one first.");

                conflict.OrderByInit();
                if (conflict.Next()) await ReplyAsync("", false, Print(conflict));
                await ReplyAsync("", false, SimplePrint($"It is now {conflict.GetCurrent().Name}'s turn to act."));
                var state = conflict.GetCurrentState();
                if (state != "") await ReplyAsync($"{conflict.GetCurrent().Name} is currently {state}.");
                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Could not move the conflict along: {e.Message}");
                Console.WriteLine($"Could not move the conflict along: {e.Message}\n{e.StackTrace}");
            }
        }

        [Command("strife")]
        [Summary("Modifies a participant's strife")]
        public async Task Strife([Summary("Number of the target on which strife must be modified")] int target,
                                  [Summary("Strife to apply (positive or negative value")] int strife)
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                var conflict = ctx.Conflicts.Where(x => x.Server.DiscordID == (long)Context.Guild.Id).OrderByDescending(x => x.ID).FirstOrDefault();
                if (conflict == null) throw new Exception($"There are no active conflicts. Create one first.");

                target -= 1;

                if (strife == 0) throw new Exception("nothing to do with strife = 0.");
                var msg = conflict.ModifyStrife(ctx.Users.Single(x => x.DiscordID == (long)Context.User.Id).ID, target, strife);
                await ReplyAsync("", false, SimplePrint(msg));

                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Could not modify strife: {e.Message}");
                Console.WriteLine($"Could not modify strife: {e.Message}\n{e.StackTrace}");
            }
        }

        [Command("fatigue")]
        [Summary("Modifies a participant's fatigue")]
        public async Task Fatigue([Summary("Number of the target on which fatigue must be modified")] int target,
                                  [Summary("Fatigue to apply (positive or negative value")] int fatigue)
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                var conflict = ctx.Conflicts.Where(x => x.Server.DiscordID == (long)Context.Guild.Id).OrderByDescending(x => x.ID).FirstOrDefault();
                if (conflict == null) throw new Exception($"There are no active conflicts. Create one first.");

                target -= 1;

                if (fatigue == 0) throw new Exception("nothing to do with fatigue = 0.");
                var msg = conflict.ModifyFatigue(ctx.Users.Single(x => x.DiscordID == (long)Context.User.Id).ID, target, fatigue);
                await ReplyAsync("", false, SimplePrint(msg));

                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Could not modify fatigue: {e.Message}");
                Console.WriteLine($"Could not modify fatigue: {e.Message}\n{e.StackTrace}");
            }
        }

        [Command("remove")]
        [Summary("Removes a participant from the conflict")]
        public async Task Remove([Summary("Number of the targets to remove from the conflict")] List<int> targets)
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                var conflict = ctx.Conflicts.Where(x => x.Server.DiscordID == (long)Context.Guild.Id).OrderByDescending(x => x.ID).FirstOrDefault();
                if (conflict == null) throw new Exception($"There are no active conflicts. Create one first.");

                var msg = "";
                foreach (var i in targets)
                {
                    msg += conflict.RemoveParticipant(i) + "\n";
                }
                await ReplyAsync("", false, SimplePrint(msg));

                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Could not remove participant(s): {e.Message}");
                Console.WriteLine($"Could not remove participant(s): {e.Message}\n{e.StackTrace}");
            }
        }

        [Command("print")]
        [Summary("Prints the conflict's state")]
        public async Task PrintConflict()
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                var conflict = ctx.Conflicts.Where(x => x.Server.DiscordID == (long)Context.Guild.Id).OrderByDescending(x => x.ID).FirstOrDefault();
                if (conflict == null) throw new Exception($"There are no active conflicts. Create one first.");

                await ReplyAsync("", false, Print(conflict));
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Could not print the conflict: {e.Message}");
                Console.WriteLine($"Could not print the conflict: {e.Message}\n{e.StackTrace}");
            }
        }

        private Embed SimplePrint(string msg)
        {
            var emd = new EmbedBuilder();

            emd.WithThumbnailUrl("https://gamepedia.cursecdn.com/l5r_gamepedia_en/thumb/8/86/Military.png/120px-Military.png");
            emd.WithDescription(msg);
            emd.WithColor(new Color(188, 131, 102));

            return emd.Build();
        }

        private Embed Print(Conflict conflict)
        {
            string msg = "";
            string late = "";
            conflict.OrderByInit();
            for (int i = 0; i < conflict.Participants.Count; i++)
            {
                var p = conflict.Participants.ElementAt(i);

                if (i == conflict.CurrentParticipant) msg += "__";
                var state = conflict.GetCurrentState(i);

                if (!p.IsLate)
                    msg += $"{i + 1} - **{p.Name}** (init = {p.Init}){(state != "" ? $" - {state}" : "")}";
                else
                    late += $"{i + 1} - **{p.Name}** (init = {p.Init}){(state != "" ? $" - {state}" : "")}";

                if (i == conflict.CurrentParticipant) msg += "__";
                msg += "\n";
            }

            if (late != "")
                msg += $"\nJoining at the beginning of the next round:\n{late}";

            var emd = new EmbedBuilder();

            emd.WithTitle($"{conflict.ConflictType.Name} - Round {conflict.Round + 1}");
            emd.WithThumbnailUrl("https://gamepedia.cursecdn.com/l5r_gamepedia_en/thumb/8/86/Military.png/120px-Military.png");
            emd.WithDescription(msg);
            emd.WithColor(new Color(188, 131, 102));

            return emd.Build();
        }
    }
}
