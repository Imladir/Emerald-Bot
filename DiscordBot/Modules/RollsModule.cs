using Discord;
using Discord.Commands;
using Discord.WebSocket;
using EmeraldBot.Bot.Tools;
using EmeraldBot.Model;
using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Game;
using EmeraldBot.Model.Identity;
using EmeraldBot.Model.Rolls;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Modules
{
    public class Rolls : ModuleBase<SocketCommandContext>
    {
        private static readonly int RollingTime = 1000;

        [Command("roll")]
        [Alias("r")]
        [Summary("Performs a free roll")]
        public async Task FreeRoll([Summary("\nDice to roll, such as 2s,3r or wwrrr.")] List<DieFace> diceDefinition,
                                   [Summary("\nReason for the roll. Include $alias or $\"Character Name\" if you're rolling for another character than your default one.")]
                                   [Remainder] CommandOptions<Character> options = null)
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                options ??= new CommandOptions<Character>(Context);
                options.Reattach(ctx);
                string reason = "";
                int tn = 0;
                if (options != null)
                {
                    if (options.Params.ContainsKey("tn")) tn = int.Parse(options.Params["tn"]);
                    reason = options.Text;
                }

                var server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id);
                var roll = new Roll()
                {
                    Character = options.Target,
                    Name = reason,
                    Server = server,
                    TN = tn,
                    DiscordChannelID = server.DiceChannelID,
                    Player = ctx.Users.Single(x => x.DiscordID == (long)Context.User.Id),
                    Initial = true
                };
                var printData = roll.Add(ctx, diceDefinition);

                var messageID = await Print(printData);
                roll.DiscordMessageID = messageID;

                Thread.Sleep(RollingTime);

                printData = roll.EndRoll();
                await Print(printData);

                roll.Character.Rolls.Add(roll);
                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Failed to roll: {e.Message}");
                Console.WriteLine($"Failed to roll: {e.Message}:\n{e.StackTrace}");
            }
        }

        [Command("secretroll")]
        [Alias("sr")]
        [Summary("Performs a free secret roll, sent to your private channel")]
        public async Task SecretRoll([Summary("\nDice to roll, such as 2s,3r or wwrrr.")] List<DieFace> diceDefinition,
                                     [Summary("\nReason for the roll. Include $alias or $\"Character Name\" if you're rolling for another character than your default one.")]
                                     [Remainder] CommandOptions<Character> options = null)
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                options ??= new CommandOptions<Character>(Context);
                options.Reattach(ctx);
                int tn = 0;
                if (options.Params.ContainsKey("tn")) tn = int.Parse(options.Params["tn"]);

                var server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id);
                var player = ctx.Users.Single(x => x.DiscordID == (long)Context.User.Id);
                var roll = new Roll()
                {
                    Character = options.Target,
                    Name = options.Text,
                    Server = server,
                    TN = tn,
                    DiscordChannelID = player.PrivateChannels.Single(x => x.Server == server).ChannelDiscordID,
                    Player = player,
                    Initial = true
                };
                var printData = roll.Add(ctx, diceDefinition);

                var messageID = await Print(printData);
                roll.DiscordMessageID = messageID;

                Thread.Sleep(RollingTime);

                printData = roll.EndRoll();
                await Print(printData);

                roll.Character.Rolls.Add(roll);
                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Failed to roll: {e.Message}");
                Console.WriteLine($"Failed to roll: {e.Message}:\n{e.StackTrace}");
            }
        }

        [Command("roll")]
        [Alias("r")]
        [Summary("Performs a skill roll")]
        public async Task RollSkill([Summary("\nSkill to roll.")] Skill s,
                               [Summary("Ring to use for the roll")] Ring r,
                               [Summary("\nReason for the roll. Include $alias or $\"Character Name\" if you're rolling for another character than your default one.")]
                               [Remainder] CommandOptions<Character> options = null)
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                options ??= new CommandOptions<Character>(Context);
                options.Reattach(ctx);
                int tn = 0;
                if (options.Params.ContainsKey("tn")) tn = int.Parse(options.Params["tn"]);

                string reason = $"{s.Name} ({r.Name})";
                if (options.Text != "") reason += $": {options.Text}";

                var server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id);
                var roll = new Roll()
                {
                    Character = options.Target,
                    Name = reason,
                    Server = server,
                    TN = tn,
                    DiscordChannelID = server.DiceChannelID,
                    Player = ctx.Users.Single(x => x.DiscordID == (long)Context.User.Id),
                    Skill = s,
                    Ring = r,
                    Initial = true
                };

                //options.Target.LoadRings(ctx);
                int ringLevel = options.Target.Rings.Single(x => x.Ring.ID == r.ID).Value;
                var skillLevel = GetSkillLevel(options.Target, s);


                var printData = roll.Add(ctx, GetDice(ringLevel, skillLevel));

                var messageID = await Print(printData);
                roll.DiscordMessageID = messageID;

                Thread.Sleep(RollingTime);

                printData = roll.EndRoll();
                await Print(printData);

                roll.Character.Rolls.Add(roll);
                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Failed to roll: {e.Message}");
                Console.WriteLine($"Failed to roll: {e.Message}:\n{e.StackTrace}");
            }
        }

        [Priority(1)]
        [Command("roll")]
        [Alias("r")]
        [Summary("Activates a technique")]
        public async Task RollTechnique([Summary("\nTechnique to roll.")] Technique t,
                                        [Summary("\nReason for the roll. Include $alias or $\"Character Name\" if you're rolling for another character than your default one.")]
                                        [Remainder] CommandOptions<Character> options = null)
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                options ??= new CommandOptions<Character>(Context);
                t = ctx.Techniques.Find(t.ID);
                // We want a technique with only one skill and the ring defined.
                if (t.Skills.Count > 1 || t.SkillGroups.Count > 0)
                    throw new Exception($"Multiple skills can be used to activate the technique. Specify one after the technique's name.");
                if (t.Skills.Count == 0 && t.SkillGroups.Count == 0)
                    throw new Exception($"There are no skills defined for the technique. You need to specify one after the technique's name");
                if (t.Ring == ctx.Rings.Single(x => x.Name.Equals("Any")))
                    throw new Exception($"Technique {t.Name} doesn't specify a ring. Add it after the technique.");

                int tn = t.TN;
                if (options.Params.ContainsKey("tn")) tn = int.Parse(options.Params["tn"]);

                string reason = $"{t.Name} ({t.Ring.Name})";
                if (options.Text != "") reason += $": {options.Text}";

                await RollTechnique(ctx, options.Target, t.Ring, t.Skills.First().Skill, tn, reason);
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Failed to roll: {e.Message}");
                Console.WriteLine($"Failed to roll: {e.Message}:\n{e.StackTrace}");
            }
        }

        [Priority(2)]
        [Command("roll")]
        [Alias("r")]
        [Summary("Activates a technique")]
        public async Task RollTechnique([Summary("\nTechnique to roll.")] Technique t,
                                        [Summary("Ring to use for the roll")] Ring r,
                                        [Summary("\nReason for the roll. Include $alias or $\"Character Name\" if you're rolling for another character than your default one.")]
                                        [Remainder] CommandOptions<Character> options = null)
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                options ??= new CommandOptions<Character>(Context);
                t = ctx.Techniques.Find(t.ID);
                // We want a technique with only one skill and the ring defined.
                if (t.Skills.Count > 1 || t.SkillGroups.Count > 0)
                    throw new Exception($"Multiple skills can be used to activate the technique. Specify one after the technique's name.");
                if (t.Skills.Count == 0 && t.SkillGroups.Count == 0)
                    throw new Exception($"There are no skills defined for the technique. You need to specify one after the technique's name");
                if (t.Ring != ctx.Rings.Single(x => x.Name.Equals("Any")) && t.Ring != r)
                    throw new Exception($"Technique {t.Name} already specifies {t.Ring.Name} as the ring.");

                int tn = t.TN;
                if (options.Params.ContainsKey("tn")) tn = int.Parse(options.Params["tn"]);

                string reason = $"{t.Name} ({r.Name})";
                if (options.Text != "") reason += $": {options.Text}";

                await RollTechnique(ctx, options.Target, r, t.Skills.First().Skill, tn, reason);
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Failed to roll: {e.Message}");
                Console.WriteLine($"Failed to roll: {e.Message}:\n{e.StackTrace}");
            }
        }

        [Priority(3)]
        [Command("roll")]
        [Alias("r")]
        [Summary("Activates a technique")]
        public async Task RollTechnique([Summary("\nTechnique to roll.")] Technique t,
                                        [Summary("Skill to use for the roll")] Skill s,
                                        [Summary("\nReason for the roll. Include $alias or $\"Character Name\" if you're rolling for another character than your default one.")]
                                        [Remainder] CommandOptions<Character> options = null)
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                options ??= new CommandOptions<Character>(Context);
                t = ctx.Techniques.Find(t.ID);
                // We want a technique with more than one skill and the ring defined.
                if (t.Skills.Count == 1 && t.SkillGroups.Count == 0)
                    throw new Exception($"The technique already specifies {t.Skills.First().Skill.Name} as the technique's skill. You must not specify another one.");
                if (t.Ring == ctx.Rings.Single(x => x.Name.Equals("Any")))
                    throw new Exception($"Technique {t.Name} doesn't specify a ring. Add it after the technique.");

                var possibleSkills = t.Skills.Select(x => x.Skill).Concat(ctx.Skills.Where(x => t.SkillGroups.Any(y => y.SkillGroup == x.Group)));
                if (!possibleSkills.ToList().Select(x => x.ID).Contains(s.ID))
                    throw new Exception($"{s.Name} is not a skill allowed to activate the technique. They are {(string.Join(", ", possibleSkills.OrderBy(x => x.Name).Select(x => x.Name)))}");

                int tn = t.TN;
                if (options.Params.ContainsKey("tn")) tn = int.Parse(options.Params["tn"]);

                string reason = $"{t.Name} ({s.Name} - {t.Ring.Name})";
                if (options.Text != "") reason += $": {options.Text}";

                await RollTechnique(ctx, options.Target, t.Ring, s, tn, reason);
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Failed to roll: {e.Message}");
                Console.WriteLine($"Failed to roll: {e.Message}:\n{e.StackTrace}");
            }
        }

        [Priority(4)]
        [Command("roll")]
        [Alias("r")]
        [Summary("Activates a technique")]
        public async Task RollTechnique([Summary("\nTechnique to roll.")] Technique t,
                                        [Summary("Skill to use for the roll")] Skill s,
                                        [Summary("Ring to use for the roll")] Ring r,
                                        [Summary("\nReason for the roll. Include $alias or $\"Character Name\" if you're rolling for another character than your default one.")]
                                        [Remainder] CommandOptions<Character> options = null)
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                options ??= new CommandOptions<Character>(Context);
                t = ctx.Techniques.Find(t.ID);
                // We want a technique with only one skill and the ring defined.
                if (t.Skills.Count == 1 && t.SkillGroups.Count == 0)
                    throw new Exception($"The technique already specifies {t.Skills.First().Skill.Name} as the technique's skill. You must not specify another one.");
                if (t.Ring != ctx.Rings.Single(x => x.Name.Equals("Any")) && t.Ring != r)
                    throw new Exception($"Technique {t.Name} already specifies {t.Ring.Name} as the ring.");

                var possibleSkills = t.Skills.Select(x => x.Skill).Concat(ctx.Skills.Where(x => t.SkillGroups.Any(y => y.SkillGroup == x.Group)));
                if (!possibleSkills.ToList().Select(x => x.ID).Contains(s.ID))
                    throw new Exception($"{s.Name} is not a skill allowed to activate the technique. They are {(string.Join(", ", possibleSkills.OrderBy(x => x.Name).Select(x => x.Name)))}");

                int tn = t.TN;
                if (options.Params.ContainsKey("tn")) tn = int.Parse(options.Params["tn"]);

                string reason = $"{t.Name} ({s.Name} - {r.Name})";
                if (options.Text != "") reason += $": {options.Text}";

                await RollTechnique(ctx, options.Target, r, s, tn, reason);
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Failed to roll: {e.Message}");
                Console.WriteLine($"Failed to roll: {e.Message}:\n{e.StackTrace}");
            }
        }

        private async Task RollTechnique(EmeraldBotContext ctx, Character c, Ring r, Skill s, int TN, string reason)
        {
            c = ctx.Characters.Find(c.ID);

            var server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id);
            var roll = new Roll()
            {
                Character = c,
                Name = reason,
                Server = server,
                TN = TN,
                DiscordChannelID = server.DiceChannelID,
                Player = ctx.Users.Single(x => x.DiscordID == (long)Context.User.Id),
                Skill = s,
                Ring = r,
                Initial = true
            };

            int ringLevel = c.Rings.Single(x => x.Ring.ID == r.ID).Value;
            int skillLevel = GetSkillLevel(c, s);


            var printData = roll.Add(ctx, GetDice(ringLevel, skillLevel));

            var messageID = await Print(printData);
            roll.DiscordMessageID = messageID;

            Thread.Sleep(RollingTime);

            printData = roll.EndRoll();
            roll.Character.Rolls.Add(roll);
            await Print(printData);

            ctx.SaveChanges();

        }

        private int GetSkillLevel(Character c, Skill s)
        {
            try
            {
                if (c is PC pc)
                {
                    return pc.Skills.Single(x => x.Skill.ID == s.ID).Value;
                }
                else if (c is NPC npc)
                {
                    return npc.SkillGroups.Single(x => x.SkillGroup.ID == s.Group.ID).Value;
                }
            } catch (Exception)
            {
                // If we fall, the skill / group wasn't found: its level is at zero.
                return 0;
            }
            return 0; // Shouldn't happen...
        }

        private List<DieFace> GetDice(int ring, int skill)
        {
            using var ctx = new EmeraldBotContext();
            var res = Enumerable.Repeat(ctx.DieFaces.Include(x => x.Emote).Single(x => x.Value.Equals("") && x.DieType.Equals("Ring")), ring).ToList();
            res.AddRange(Enumerable.Repeat(ctx.DieFaces.Include(x => x.Emote).Single(x => x.Value.Equals("") && x.DieType.Equals("Skill")), skill));

            return res;
        }

        [Command("add")]
        [Summary("Adds dice to the last roll performed by the player")]
        public async Task Add([Summary("\nDice to add to the roll, each separated by a comma. In addition to the rolled dice (see command **roll**) \n" +
                                       "it is possible to add non-rolled dice with predefined results.\n" +
                                       "- The first character defines the type of the die (r or b for ring dice, s or w for skill dice)\n" +
                                       "If their definition stops there, the die will be rolled. To defined a pre-set die, add one or two characters:\n" +
                                       "-- s for success,\n" +
                                       "-- o for opportunity,\n" +
                                       "-- t for strife,\n" +
                                       "-- e for explosive success.\n" +
                                       "Those characters can be combined (in any order) to create a result with two symbols. For example, " +
                                       "a skill die with Explosive Success + Strife would be defined as *set* (or *ste*).")] List<DieFace> diceDefinition,
                                       [Summary("\nThe operation will be logged, and if **reason=\"xxx\"** is provided, it will be added to the log.")]
                                       [Remainder] string reason = "")
        {
            try
            {
                using var ctx = new EmeraldBotContext();
                var roll = ctx.Rolls.Where(x => x.Server.DiscordID == (long)Context.Guild.Id
                                             && x.Player.DiscordID == (long)Context.User.Id)
                                    .OrderByDescending(x => x.LastUpdated).FirstOrDefault();
                ctx.Entry(roll).Collection(x => x.Dice).Query()
                    .Include(x => x.Face).ThenInclude(x => x.Emote);
                ctx.Characters.Attach(roll.Character);
                if (roll == null) throw new Exception("No roll found to modify.");

                var printData = roll.Add(ctx, diceDefinition, reason);
                await Print(printData);

                if (printData.IsRolling)
                {
                    Thread.Sleep(RollingTime);
                    await Print(roll.EndRoll());
                }
                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e) {
                await ReplyAsync($"Failed to add dice: {e.Message}");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        [Command("keep")]
        [Alias("k")]
        [Summary("Keep some dice and drop the others.")]
        public async Task Keep([Summary("\nIndices of the dice to keep, separated by a coma or semi-colon, *i.e.* 1,4,5.")] List<int> indices,
                                   [Remainder]
                                       [Summary("\nThe operation will be logged, and if a reason is provided, it will be added to the log.")]
                                       string reason = "")
        {
            try
            {
                using var ctx = new EmeraldBotContext();
                var roll = ctx.Rolls.Where(x => x.Server.DiscordID == (long)Context.Guild.Id
                                             && x.Player.DiscordID == (long)Context.User.Id)
                                    .OrderByDescending(x => x.LastUpdated).FirstOrDefault();
                ctx.Entry(roll).Collection(x => x.Dice).Query()
                    .Include(x => x.Face).ThenInclude(x => x.Emote);
                ctx.Characters.Attach(roll.Character);
                if (roll == null) throw new Exception("No roll found to modify.");

                await Print(roll.Keep(ctx, indices, reason));

                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Failed to keep dice: {e.Message}");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        [Command("drop")]
        [Alias("d")]
        [Summary("drop some dice and keep the others.")]
        public async Task Drop([Summary("\nIndices of the dice to drop, separated by a coma or semi-colon, *i.e.* 1,4,5.")] List<int> indices,
                               [Remainder]
                               [Summary("\nThe operation will be logged, and if a reason is provided, it will be added to the log.")]
                               string reason = "")
        {
            try
            {
                using var ctx = new EmeraldBotContext();
                var roll = ctx.Rolls.Where(x => x.Server.DiscordID == (long)Context.Guild.Id
                                             && x.Player.DiscordID == (long)Context.User.Id)
                                    .OrderByDescending(x => x.LastUpdated).FirstOrDefault();
                ctx.Entry(roll).Collection(x => x.Dice).Query()
                    .Include(x => x.Face).ThenInclude(x => x.Emote);
                ctx.Characters.Attach(roll.Character);
                if (roll == null) throw new Exception("No roll found to modify.");

                await Print(roll.Drop(ctx, indices, reason));

                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Failed to keep dice: {e.Message}");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        [Command("replace")]
        [Summary("Replaces one or more dice with those given.")]
        public async Task Replace([Summary("\nIndices of the dice to replace")] List<int> indices,
                                      [Summary("\nDice to add to the roll, each separated by a comma. In addition to the rolled dice (see command **roll**) \n" +
                                                  "it is possible to add non-rolled dice with predefined results.\n" +
                                                  "- The first character defines the type of the die (r or b for ring dice, s or w for skill dice)\n" +
                                                  "If their definition stops there, the die will be rolled. To defined a pre-set die, add one or two characters:\n" +
                                                  "-- s for success,\n" +
                                                  "-- o for opportunity,\n" +
                                                  "-- t for strife,\n" +
                                                  "-- e for explosive success.\n" +
                                                  "Those characters can be combined (in any order) to create a result with two symbols. For example, " +
                                                   "a skill die with Explosive Success + Strife would be defined as *set* (or *ste*).")] List<DieFace> diceDefinition,
                                      [Summary("\nThe operation will be logged, and if a reason is provided, it will be added to the log.")]
                                          [Remainder] string reason = "")
        {
            try
            {
                using var ctx = new EmeraldBotContext();
                var roll = ctx.Rolls.Where(x => x.Server.DiscordID == (long)Context.Guild.Id
                                             && x.Player.DiscordID == (long)Context.User.Id)
                                    .OrderByDescending(x => x.LastUpdated).FirstOrDefault();
                ctx.Entry(roll).Collection(x => x.Dice).Query()
                    .Include(x => x.Face).ThenInclude(x => x.Emote);
                ctx.Characters.Attach(roll.Character);
                if (roll == null) throw new Exception("No roll found to modify.");

                var printData = roll.Replace(ctx, indices, diceDefinition, reason);
                await Print(printData);

                if (printData.IsRolling)
                {
                    Thread.Sleep(RollingTime);
                    await Print(roll.EndRoll());
                }

                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                _ = await ReplyAsync($"Failed to replace dice: {e.Message}");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        [Command("reroll")]
        [Summary("Re-roll one or more dice.")]
        public async Task Reroll([Summary("\nThe only required parameter is a series of indices of dice to reroll, separated by commas.\n" +
                                                  "Saying for example **!reroll 2,3,4** will reroll the dice 2, 3 and 4 and ignore all the others.")]
                                          List<int> indices,
                                 [Summary("The operation will be logged, if a reason is provided, it will be added to the log.")]
                                 [Remainder] string reason = "")
        {
            try
            {
                using var ctx = new EmeraldBotContext();
                var roll = ctx.Rolls.Where(x => x.Server.DiscordID == (long)Context.Guild.Id
                                             && x.Player.DiscordID == (long)Context.User.Id)
                                    .OrderByDescending(x => x.LastUpdated).FirstOrDefault();
                ctx.Entry(roll).Collection(x => x.Dice).Query()
                    .Include(x => x.Face).ThenInclude(x => x.Emote);
                ctx.Characters.Attach(roll.Character);
                if (roll == null) throw new Exception("No roll found to modify.");

                await Print(roll.Reroll(ctx, indices, reason));

                Thread.Sleep(RollingTime);

                _ = await Print(roll.EndRoll());

                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Failed to explode dice: {e.Message}");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        [Command("explode")]
        [Alias("e")]
        [Summary("Explodes dice.\nNew dice are added and rolled as needed. An already exploded die can't explode again.")]
        public async Task Explode()
        {
            try
            {
                using var ctx = new EmeraldBotContext();
                var roll = ctx.Rolls.Where(x => x.Server.DiscordID == (long)Context.Guild.Id
                                             && x.Player.DiscordID == (long)Context.User.Id)
                                    .OrderByDescending(x => x.LastUpdated).FirstOrDefault();
                ctx.Entry(roll).Collection(x => x.Dice).Query()
                    .Include(x => x.Face).ThenInclude(x => x.Emote);
                ctx.Characters.Attach(roll.Character);
                if (roll == null) throw new Exception("No roll found to modify.");

                await Print(roll.Explode(ctx));

                Thread.Sleep(RollingTime);

                await Print(roll.EndRoll());

                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Failed to explode dice: {e.Message}");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        [Command("settn")]
        [Alias("tn")]
        [Summary("Set or resets the TN of the last roll")]
        public async Task SetTN([Remainder]
                                    [Summary("\nThe TN of the roll.")] int tn)
        {
            try
            {
                using var ctx = new EmeraldBotContext();
                var roll = ctx.Rolls.Where(x => x.Server.DiscordID == (long)Context.Guild.Id
                                             && x.Player.DiscordID == (long)Context.User.Id)
                                    .OrderByDescending(x => x.LastUpdated).FirstOrDefault();
                ctx.Entry(roll).Collection(x => x.Dice).Query()
                    .Include(x => x.Face).ThenInclude(x => x.Emote);
                ctx.Characters.Attach(roll.Character);
                if (roll == null) throw new Exception("No roll found to modify.");

                await Print(roll.SetTN(ctx, tn));

                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Failed to set TN: {e.Message}");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        [Command("rollrename")]
        [Alias("name")]
        [Summary("Sets or renames the current active roll.")]
        public async Task SetName([Remainder]
                                      [Summary("\nThe name of the roll.")]
                                      string name)
        {
            try
            {
                using var ctx = new EmeraldBotContext();
                var roll = ctx.Rolls.Where(x => x.Server.DiscordID == (long)Context.Guild.Id
                                             && x.Player.DiscordID == (long)Context.User.Id)
                                    .OrderByDescending(x => x.LastUpdated).FirstOrDefault();
                ctx.Entry(roll).Collection(x => x.Dice).Query()
                    .Include(x => x.Face).ThenInclude(x => x.Emote);
                ctx.Characters.Attach(roll.Character);
                if (roll == null) throw new Exception("No roll found to modify.");

                await Print(roll.SetName(ctx, name));

                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Failed to rename roll: {e.Message}");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        //    [Command("findopportunities")]
        //    [Alias("opp")]
        //    [Summary("Tries to find the opportunities that apply to the current roll")]
        //    public async Task FindOpportunties()
        //    {
        //        var identity = new UserIdentity(Context.Guild.Id, Context.User.Id);

        //        try
        //        {
        //            if (Finder.Player(identity).PrivateChannelID == 0)
        //                throw new BotException("You must define your private channel before you're able to use this command. (see **!setPrivateChannel**)");

        //            var targetChan = Context.Client.GetChannel(Finder.Player(identity).PrivateChannelID) as ISocketMessageChannel;

        //            string msg = "";
        //            RollData roll = Finder.Roller(identity.ServerID).GetRoll(identity.UserID);
        //            var opps = roll.FindOpportunities();
        //            if (opps.Count > 0)
        //            {

        //                foreach (var op in opps.OrderBy(x => x.RequiredAmount).ThenBy(x => x.Variable).ThenBy(x => x.Source))
        //                {
        //                    var str = "";
        //                    for (int i = 0; i < op.RequiredAmount; i++) str += "{opportunity}";
        //                    str = $"{str}{(op.Variable ? "+" : "")}: {op.Effect}".ReplaceSymbols();
        //                    if (str.Count() + msg.Count() < 2000) msg += $"{str}\n";
        //                    else
        //                    {
        //                        await targetChan.SendMessageAsync(msg);
        //                        msg = $"{str}\n";
        //                    }
        //                }
        //                if (msg != "") await targetChan.SendMessageAsync(msg);
        //            }
        //            else
        //            {
        //                await ReplyAsync("I didn't find any matching opportunities.");
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine($"Error while trying to find opportunities: {e.Message}\n{e.StackTrace}");
        //            await ReplyAsync($"Could not find opportunities: {e.Message}");
        //        }
        //    }

        [Command("lock")]
        [Alias("l")]
        [Summary("Locks a roll")]
        public async Task LockRoll()
        {

            try
            {
                using var ctx = new EmeraldBotContext();
                var roll = ctx.Rolls.Where(x => x.Server.DiscordID == (long)Context.Guild.Id
                                             && x.Player.DiscordID == (long)Context.User.Id)
                                    .OrderByDescending(x => x.LastUpdated).FirstOrDefault();
                ctx.Entry(roll).Collection(x => x.Dice).Query()
                    .Include(x => x.Face).ThenInclude(x => x.Emote);
                ctx.Characters.Attach(roll.Character);
                if (roll == null) throw new Exception("No roll found to modify.");

                var msg = roll.Lock(ctx).ReplaceSymbols(Context.Guild.Id);

                List<ulong> sent = new List<ulong>();
                foreach (var gm in ctx.GetGM(Context.Guild.Id))
                {
                    var chanID = ctx.PrivateChannels.SingleOrDefault(x => x.Player.ID == gm.ID && x.Server.DiscordID == (long)Context.Guild.Id);
                    if (chanID != null && !sent.Contains((ulong)chanID.ChannelDiscordID)) {
                        var chan = Context.Guild.GetChannel((ulong)chanID.ChannelDiscordID) as ISocketMessageChannel;
                        await chan.SendMessageAsync(msg);
                        sent.Add(chan.Id);
                    }
                }
                ctx.SaveChanges();
                await ReplyAsync("Roll has been locked.");
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Failed to keep dice: {e.Message}");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        private async Task<long> Print(RollPrintData data)
        {
            var emd = new EmbedBuilder();
            emd.WithTitle(data.Title);
            emd.AddField("Roll", data.Dice.ReplaceSymbols(Context.Guild.Id));
            emd.AddField("Result", data.Result.ReplaceSymbols(Context.Guild.Id));

            var logList = data.Log.FitDiscordMessageSize(1024);
            for (int i = 0; i < logList.Count; i++)
            {
                if (i == 0) emd.AddField("Log", logList[i].ReplaceSymbols(Context.Guild.Id));
                else emd.AddField("Log (continued)", logList[i].ReplaceSymbols(Context.Guild.Id));
            }
            if (data.Icon != "") emd.WithThumbnailUrl(data.Icon);

            Color color = Color.LightGrey;
            if (data.State == 1) color = Color.Green;
            else if (data.State == -1) color = Color.Red;
            emd.WithColor(color);

            ulong channelID = (ulong)data.ChannelID;
            if (channelID == 0) channelID = Context.Channel.Id;

            var channel = Context.Client.GetChannel(channelID) as ISocketMessageChannel;

            long res = data.MessageID;
            if (res == 0)
            {
                // It's a new roll, send a new message
                var message = await channel.SendMessageAsync("", false, emd.Build());
                res = (long)message.Id;
            }
            else
            {
                // Retrieve previous roll and modify it
                IUserMessage msg = (IUserMessage)await channel.GetMessageAsync((ulong)res);
                await msg.ModifyAsync(x => x.Embed = emd.Build());
            }

            return res;
        }
    }
}
