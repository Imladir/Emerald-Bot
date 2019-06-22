using Discord;
using Discord.Commands;
using EmeraldBot.Bot.Tools;
using EmeraldBot.Model;
using EmeraldBot.Model.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Modules
{
    public class Characters : ModuleBase<SocketCommandContext>
    {

        [Command("create")]
        [Summary("Creates a new character")]
        public async Task CreateCharacter([Summary("Alias of the character to create")] string alias,
                                          [Summary("key=value pairs containing more informations about the character.\n" +
                                                   "If a value contains more than one word, put it between \"\".\n" +
                                                   "To learn more about the available fields, use the command **character fields**.")]
                                          [Remainder] CommandOptions<Character> options)
        {
            using var ctx = new EmeraldBotContext();
            using var dbTransaction = ctx.Database.BeginTransaction();
            try
            {
                var newChar = new PC()
                {
                    Alias = alias,
                    Name = alias,
                    Server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id),
                    Player = ctx.Users.Single(x => x.DiscordID == (long)Context.User.Id)
                };
                newChar.InitRings(ctx);

                ctx.Characters.Add(newChar);
                var msg = UpdateChar(ctx, newChar, options.Params);
                if (msg == "") msg = $"{newChar.Name} has been succesfully created.";
                ctx.SaveChanges();
                dbTransaction.Commit();
                await ReplyAsync(msg);
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                dbTransaction.Rollback();
                await ReplyAsync($"Couldn't create the character: {e.Message}");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        [Command("update")]
        [Alias("u")]
        [Summary("Update the data of a character")]
        public async Task Update([Remainder]
                                 [Summary("key=value pairs containing more informations about the character.\n" +
                                                    "If a value contains more than one word, put it between \"\"." +
                                                    "To learn more about the available fields, use the command **character fields**.")]
                                 CommandOptions<Character> options)
        {
            using var ctx = new EmeraldBotContext();
            using var dbTransaction = ctx.Database.BeginTransaction();
            try
            {
                // It's an update, I better load everything I can
                options.Reattach(ctx);
                //options.Target.FullLoad(ctx);

                var msg = UpdateChar(ctx, options.Target, options.Params);
                if (msg == "") msg = $"{options.Target.Name} has been succesfully updated.";
                ctx.SaveChanges();
                dbTransaction.Commit();
                await ReplyAsync(msg);
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                dbTransaction.Rollback();
                await ReplyAsync($"Couldn't update the character: {e.Message}");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        private string UpdateChar(EmeraldBotContext ctx, Character target, Dictionary<string, string> dic)
        {
            string msg = "";
            List<string> errors = new List<string>();

            foreach (var kv in dic)
            {
                if (kv.Key.Equals("add", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var aliasOrName in Regex.Split(kv.Value, "[;,]"))
                    {
                        var na = ctx.NameAliases.SingleOrDefault(x => x.Alias == aliasOrName || x.Name == aliasOrName);
                        if (na == null) errors.Add($"Couldn't find anything with name or alias '{aliasOrName}'. Check spelling.");
                        else if (!target.Add(na)) errors.Add($"Couldn't add {na.Name}. Character already has it?");
                    }
                }
                else if (kv.Key.Equals("remove", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var aliasOrName in Regex.Split(kv.Value, "[;,]"))
                    {
                        var na = ctx.NameAliases.SingleOrDefault(x => x.Alias == aliasOrName || x.Name == aliasOrName);
                        if (na == null) errors.Remove($"Couldn't find anything with name or alias '{aliasOrName}'. Check spelling.");
                        else if (!target.Remove(na)) errors.Remove($"Couldn't remove {na.Name}. Character doesn't have it?");
                    }
                }
                else
                {
                    if (!target.UpdateField(ctx, kv.Key, kv.Value)) errors.Add($"Couldn't set {kv.Key} with value {kv.Value}");
                }
            }

            if (errors.Count != 0) msg = $"Some errors occured while updating {target.Name}:\n" + String.Join("\n", errors) + "\nThe rest (if anything) was saved.";
            return msg;
        }

        [Command("fields")]
        [Summary("Displays the available fields for character creation / update")]
        public async Task Fields()
        {
            string msg = $"Possible keys for characters creations and update are: " + String.Join(", ", PC.AcceptedFields) +
                         $". All the skills and rings are also valid.\n" +
                         $"In addition, you can use learn and forget followed by one of more techniques (separated by commas or semi-colons) " +
                         $"or addavantage and removedavantage in the same way.\n\nIn every command you can use more than one key=value pair. For example:\n" +
                         $"character create satsume name=\"Doji Satsume\" clan=Crane description=\"The Emerald Champion\"\n" +
                         $"character update koei name=\"Hida Koei\" fire=2 earth=3 melee=2 survival=1 learn=\"Striking as Earth,Lord Hida's Grip\"";
            await ReplyAsync(msg);
            await Context.Channel.DeleteMessageAsync(Context.Message);
        }

        [Command("fatigue")]
        [Alias("f")]
        [Summary("Increases or decreases the current fatigue")]
        public async Task Fatigue([Summary("Amount of fatigue to add (or remove if negative amount")] int fatigue,
                                  [Summary("Name of alias of the character on whom the fatigue will be applied.")] CommandOptions<PC> options = null)
        {
            string msg = "";
            try
            {
                msg += "Fatigue applied. ";

                using (EmeraldBotContext ctx = new EmeraldBotContext())
                {
                    options ??= new CommandOptions<PC>(Context);
                    options.Reattach(ctx);

                    msg += options.Target.ModifyFatigue(ctx, fatigue);

                    ctx.SaveChanges();

                    var currentPlayer = ctx.Users.Single(x => x.DiscordID == (long)Context.User.Id);
                    ctx.Entry(options.Target).Reference(x => x.Player).Load();
                    if (ctx.CheckPrivateChannel(Context.Guild.Id, Context.User.Id, Context.Channel.Id, (ulong)options.Target.Player.DiscordID))
                        msg += $"Fatigue is now at {options.Target.Fatigue}.";

                }

                await ReplyAsync(msg);
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Couldn't update fatigue: {e.Message}");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        [Command("strife")]
        [Alias("s")]
        [Summary("Increases or decreases the current strife")]
        public async Task Strife([Summary("Amount of strife to add (or remove if negative amount")] int strife,
                                  [Summary("Name of alias of the character on whom the fatigue will be applied.")] CommandOptions<PC> options = null)
        {
            string msg = "";
            try
            {
                msg += "Strife applied. ";

                using (var ctx = new EmeraldBotContext())
                {
                    options ??= new CommandOptions<PC>(Context);
                    options.Reattach(ctx);

                    msg += options.Target.ModifyStrife(ctx, strife);

                    ctx.SaveChanges();

                    var currentPlayer = ctx.Users.Single(x => x.DiscordID == (long)Context.User.Id);
                    ctx.Entry(options.Target).Reference(x => x.Player).Load();
                    if (ctx.CheckPrivateChannel(Context.Guild.Id, Context.User.Id, Context.Channel.Id, (ulong)options.Target.Player.DiscordID))
                        msg += $"Strife is now at {options.Target.Strife}.";

                }

                await ReplyAsync(msg);
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Couldn't update strife: {e.Message}");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        [Command("void")]
        [Alias("v")]
        [Summary("Increases or decreases the current void points")]
        public async Task Void([Summary("Amount of Void points to add (or remove if negative amount")] int voidPoints,
                                  [Summary("Name of alias of the character on whom the void points will be changed.")] CommandOptions<PC> options = null)
        {
            try
            {
                var msg = "Available Void points updated. ";

                using var ctx = new EmeraldBotContext();
                options ??= new CommandOptions<PC>(Context);
                options.Reattach(ctx);
                //options.Target.LoadRings(ctx);

                int newVoid = Math.Min(Math.Max(options.Target.Ring("Void") + voidPoints, 0), options.Target.Ring("Void"));
                options.Target.CurrentVoid = newVoid;


                var currentPlayer = ctx.Users.Single(x => x.DiscordID == (long)Context.User.Id);

                //ctx.Entry(options.Target).Reference(x => x.Player).Load();
                if (ctx.CheckPrivateChannel(Context.Guild.Id, Context.User.Id, Context.Channel.Id, (ulong)options.Target.Player.DiscordID))
                    msg += $"{options.Target.Name} now has {options.Target.CurrentVoid} void point(s) available.";
                ctx.SaveChanges();
                await ReplyAsync(msg);
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Couldn't apply void points change: {e.Message}.");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        [Group("journal")]
        [Alias("j")]
        public class JournalsModule : ModuleBase<SocketCommandContext>
        {
            [Command("init")]
            [Summary("Initialises the Glory, Honour and/or Status journals with character creation values.\n" +
                     "Can only be used as long as there are no other modifications of the journals, " +
                     "but successive calls of this command are possible, they will simply replace the values.\n\n" +
                      "If you want to make it look like another character than your default one is speaking, add **$alias**")]
            public async Task Init([Remainder]
                               [Summary("Key-Value pairs in the form of key=value, where key is Glory, Status or Honour.\n" +
                                       "You can put all three values at once or just one or two.\n\n" +
                                       "If you want to modify the journals for another character than your default one, add **$alias**")]
                               CommandOptions<PC> options)
            {
                string msg = "";
                try
                {
                    using var ctx = new EmeraldBotContext();
                    options.Reattach(ctx);
                    ctx.Entry(options.Target).Collection(x => x.JournalEntries).Load();

                    foreach (var k in new List<String>() { "Glory", "Honour", "Status" })
                    {
                        if (!options.Params.ContainsKey(k.ToLower())) continue;

                        var jType = ctx.JournalTypes.Single(x => x.Name.Equals(k));

                        if (options.Target.JournalEntries.Count(x => x.Journal == jType) > 1)
                            throw new Exception($"journal {k} as already been modified post creation, {k} cannot be initialised again.");

                        var initialEntry = options.Target.JournalEntries.SingleOrDefault(x => x.Journal == jType);
                        if (initialEntry != null) initialEntry.Amount = int.Parse(options.Params[k.ToLower()]);
                        else
                        {
                            initialEntry = new JournalEntry()
                            {
                                Amount = int.Parse(options.Params[k.ToLower()]),
                                Journal = jType,
                                Reason = "Character Creation"
                            };
                            options.Target.JournalEntries.Add(initialEntry);
                        }
                        msg += $"Journal {k} successfully initialized.\n";
                    }

                    ctx.SaveChanges();

                    await ReplyAsync(msg);
                    await Context.Channel.DeleteMessageAsync(Context.Message);

                }
                catch (Exception e)
                {
                    await ReplyAsync($"Couldn't initialise journals: {e.Message}");
                    Console.WriteLine($"{e.Message}\n{e.StackTrace}");
                }
            }

            [Command("update")]
            [Alias("u")]
            [Summary("Updates a character's journals (xp, glory, status, honour).\n")]
            public async Task UpdateJournal([Summary("The journal to update")] string journal,
                                            [Summary("Amount by which you want to update the given journal")] int amount,
                                            [Summary("Reason for the update, possible $alias")][Remainder] CommandOptions<PC> options)
            {
                string msg = "";

                try
                {
                    if (amount == 0) throw new Exception("The amount must be different from 0.");
                    if (options.Text == "") throw new Exception("You need to give a reason for the journal modification");

                    using (var ctx = new EmeraldBotContext())
                    {
                        options.Reattach(ctx);
                        ctx.Entry(options.Target).Collection(x => x.JournalEntries).Load();

                        var jEntry = new JournalEntry()
                        {
                            Amount = amount,
                            Journal = ctx.JournalTypes.Single(x => x.Name.Equals(journal)),
                            Reason = options.Text
                        };
                        options.Target.JournalEntries.Add(jEntry);

                        ctx.SaveChanges();

                        msg += $"Journal {journal} updated successfully.";

                        var currentPlayer = ctx.Users.Single(x => x.DiscordID == (long)Context.User.Id);
                        ctx.Entry(options.Target).Reference(x => x.Player).Load();
                        if (ctx.CheckPrivateChannel(Context.Guild.Id, Context.User.Id, Context.Channel.Id, (ulong)options.Target.Player.DiscordID))
                            msg += $" New score: {options.Target.CurrentJournalValue(journal)}";
                    }

                    await ReplyAsync(msg);
                    await Context.Channel.DeleteMessageAsync(Context.Message);

                }
                catch (Exception e)
                {
                    await ReplyAsync($"Couldn't update journal: {e.Message}");
                    Console.WriteLine($"{e.Message}\n{e.StackTrace}");
                }
            }

            [Command("history")]
            [Alias("h")]
            [Summary("Displays or updates a character's journals (xp, glory, status, honour).\n" +
                     "The displays are only available on the private channel of the player owning the character (or on the GM's).")]
            public async Task SeeJournal([Summary("The journal to display. If left empty, will show the last five entries of each journal")] string journal = "",
                                         [Summary("The number of entries to show. 5 by default.")] int nEntries = 5,
                                         [Summary("$Name or $alias of the character whose journal you want to see")] CommandOptions<PC> options = null)
            {
                List<string> journals = new List<string>() { "xp", "glory", "honour", "status" };

                try
                {
                    options ??= new CommandOptions<PC>(Context);
                    using var ctx = new EmeraldBotContext();

                    options.Reattach(ctx);
                    var player = ctx.Users.Single(x => x.DiscordID == (long)Context.User.Id);

                    ctx.Entry(options.Target).Reference(x => x.Player).Load();
                    options.Target.LoadJournals(ctx);

                    if (!ctx.CheckPrivateChannel(Context.Guild.Id, Context.User.Id, Context.Channel.Id, (ulong)options.Target.Player.DiscordID))
                        throw new Exception($"#{Context.Channel.Name} is not a valid private channel.");

                    foreach (var j in journals)
                    {
                        if (journal == "" || j.Equals(journal, StringComparison.OrdinalIgnoreCase))
                        {
                            // retrieve the entries
                            var jEntries = options.Target.JournalEntries.Where(x => x.Journal.Name.Equals(j, StringComparison.OrdinalIgnoreCase))
                                                                   .OrderByDescending(x => x.EntryDate)
                                                                   .Take(nEntries).Reverse().ToList();
                            var score = options.Target.JournalEntries.Where(x => x.Journal.Name.Equals(j, StringComparison.OrdinalIgnoreCase)).Sum(x => x.Amount);

                            var emd = new EmbedBuilder();

                            var clan = ctx.Clans.Single(x => x.PCs.Any(y => y.ID == options.Target.ID));
                            emd.WithColor(new Color((uint)clan.Colour));
                            if (options.Target.Icon != "") emd.WithThumbnailUrl(options.Target.Icon);
                            if (jEntries.Count > 0)
                            {
                                var sum = jEntries.Sum(x => x.Amount);
                                emd.WithTitle($"{j.Capitalize()} - Latest {jEntries.Count} {(jEntries.Count > 1 ? "entries" : "entry")} - Current score: {score}");

                                string value = "";
                                foreach (var je in jEntries) value += $"{je.EntryDate.ToString("yy-MM-dd")}: {je.Amount.ToString("+#;-#")} - {je.Reason}\n";
                                emd.WithDescription(value);
                            }
                            else
                            {
                                emd.WithTitle($"{j.Capitalize()} - Current score: 0");
                                emd.WithDescription("No entries yet.");
                            }
                            await ReplyAsync("", false, emd.Build());
                        }
                    }
                    await Context.Channel.DeleteMessageAsync(Context.Message);
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Couldn't display journals: {e.Message}");
                    Console.WriteLine($"{e.Message}\n{e.StackTrace}");
                }

            }
        }
    }
}
