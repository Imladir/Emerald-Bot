using Discord.Commands;
using EmeraldBot.Bot.Tools;
using EmeraldBot.Model;
using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Game;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Modules
{
    [Group("data")]
    public class GameData : ModuleBase<SocketCommandContext>
    {
        [Group("create")]
        public class Create : ModuleBase<SocketCommandContext>
        {
            [Command("technique")]
            [Summary("Creates a new technique.")]
            public async Task CreateTechnique([Summary("\nAlias of the technique to create. It MUST be one word.")] string alias,
                                              [Summary("\nKey value pairs containing the information about the new technique.\n" +
                                                       "See **data fields technique** for more information about what those fields are.")]
                                               [Remainder] CommandOptions<Character> args)
            {
                using var ctx = new EmeraldBotContext();
                try
                {
                    ulong serverID = Context.Guild.Id;
                    if (alias.Contains(" "))
                        throw new Exception("The alias must be a single word.");
                    if (ctx.GetNameAliasEntity(serverID, alias) is NameAlias na)
                        throw new Exception($"Something with alias '{alias}' already exists: {na.Name}. Pick something else.");

                    Technique t = new Technique()
                    {
                        Alias = alias,
                        Name = alias,
                        Server = ctx.Servers.Single(x => x.DiscordID == (long)serverID)
                    };
                    t.Update(ctx, args.Params);

                    string msg = $"Technique {(t.Name != "" ? t.Name : t.Alias)} was added successfully.";
                    await ReplyAsync(msg);
                    ctx.SaveChanges();
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Failed to create technique: {e.Message}");
                    Console.WriteLine($"Exception: {e.Message}:\n{e.StackTrace}");
                }
            }
            [Command("skill")]
            [Summary("Creates a new skill.")]
            public async Task CreateSkill([Summary("\nAlias of the skill to create. It MUST be one word.")] string alias,
                                          [Summary("\nName of the skill to create")] string name,
                                          [Summary("\nGroup to which the skill belongs to.")] string group,
                                          [Summary("\nThe *Book, page* in which the skill can be found")] string source = "")
            {
                using var ctx = new EmeraldBotContext();
                try
                {
                    ulong serverID = Context.Guild.Id;
                    if (alias.Contains(" "))
                        throw new Exception("The alias must be a single word.");
                    if (ctx.GetNameAliasEntity(serverID, alias) is NameAlias na)
                        throw new Exception($"Something with alias '{alias}' already exists: {na.Name}. Pick something else.");
                    var skillGroup = ctx.SkillGroups.SingleOrDefault(x => x.Name.Equals(group));
                    if (skillGroup == null) throw new Exception($"Couldn't find skill group {group}. Skill groups are {String.Join(", ", ctx.Set<SkillGroup>().Select(x => x.Name))}.");

                    Skill s = new Skill()
                    {
                        Alias = alias,
                        Name = name,
                        Server = ctx.Servers.Single(x => x.DiscordID == (long)serverID),
                        Group = skillGroup
                    };
                    if (source != "")
                    {
                        var split = source.Split(',');
                        s.Source = new Source() { Book = split[0], Page = int.TryParse(split[1], out int v) ? v : -1 };
                    }

                    string msg = $"Skill {s.Name} was added successfully.";
                    await ReplyAsync(msg);
                    ctx.SaveChanges();
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Failed to create skill: {e.Message}");
                    Console.WriteLine($"Exception: {e.Message}:\n{e.StackTrace}");
                }

            }

            [Command("advantage")]
            [Summary("Creates a new advantage.")]
            public async Task CreateAdvantage([Summary("\nAlias of the advantage to create. It MUST be one word.")] string alias,
                                              [Summary("\nClass of advantage to create: distinction, passion, adversity or anxiety")] string advClass,
                                              [Summary("\nKey value pairs containing the information about the new advantage.\n" +
                                                       "See **data fields advantage** for more information about what those fields are.")]
                                               [Remainder] CommandOptions<Character> args)
            {
                using var ctx = new EmeraldBotContext();
                try
                {
                    ulong serverID = Context.Guild.Id;
                    if (alias.Contains(" "))
                        throw new Exception("The alias must be a single word.");
                    if (ctx.GetNameAliasEntity(serverID, alias) is NameAlias na)
                        throw new Exception($"Something with alias '{alias}' already exists: {na.Name}. Pick something else.");

                    Advantage a = new Advantage()
                    {
                        Alias = alias,
                        Name = alias,
                        Server = ctx.Servers.Single(x => x.DiscordID == (long)serverID)
                    };
                    a.Update(ctx, args.Params);

                    string msg = $"Advantage {(a.Name != "" ? a.Name : a.Alias)} was added successfully.";
                    await ReplyAsync(msg);
                    ctx.SaveChanges();
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Failed to create advantage: {e.Message}");
                    Console.WriteLine($"Exception: {e.Message}:\n{e.StackTrace}");
                }

            }
            [Command("npc")]
            [Summary("Creates a new NPC.")]
            public async Task CreateNPC([Summary("\nAlias of the NPC to create. It MUST be one word.")] string alias,
                                              [Summary("\nKey value pairs containing the information about the new advantage.\n" +
                                                       "See **data fields NPC** for more information about what those fields are.")]
                                               [Remainder] CommandOptions<Character> args)
            {
                using var ctx = new EmeraldBotContext();
                using var dbTransaction = ctx.Database.BeginTransaction();
                try
                {
                    var newChar = new NPC()
                    {
                        Alias = alias,
                        Name = alias,
                        Server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id)
                    };
                    newChar.InitRings(ctx);

                    ctx.Characters.Add(newChar);
                    newChar.Update(ctx, args.Params);
                    ctx.SaveChanges();
                    dbTransaction.Commit();
                    await ReplyAsync($"NPC {(newChar.Name != "" ? newChar.Name : newChar.Alias)} was added successfully.");
                }
                catch (Exception e)
                {
                    dbTransaction.Rollback();
                    await ReplyAsync($"Couldn't create the character: {e.Message}");
                    Console.WriteLine($"{e.Message}\n{e.StackTrace}");
                }
            }
        }


        [Group("fields")]
        public class Fields : ModuleBase<SocketCommandContext>
        {

            [Command("advantage")]
            [Summary("Displays the available fields for advantage creation / update")]
            public async Task FieldsAdvantage()
            {
                string msg = $"Possible keys for characters creations and update are: {String.Join(", ", Advantage.AllowedFields)}.";
                await ReplyAsync(msg);
            }

            [Command("technique")]
            [Summary("Displays the available fields for technique creation / update")]
            public async Task FieldsTechnique()
            {
                string msg = $"Possible keys for characters creations and update are: {String.Join(", ", Technique.AllowedFields)}.";
                msg += "\nNote that for skills, it can contain multiple ones separated by a comma or semi-colon. It can include indiferently skills and skill groups.";
                msg += "\nExample: data update technique flySparrow name=\"Fly of the Sparrow\" skill=martial,composition";

                await ReplyAsync(msg);
            }

            [Command("npc")]
            [Summary("Displays the available fields for NPC creation / update")]
            public async Task FieldsNPC()
            {
                string msg = $"Possible keys for characters creations and update are: {String.Join(", ", NPC.AcceptedFields)}.";
                await ReplyAsync(msg);
            }

        }

        [Group("update")]
        [Alias("u")]
        public class Update : ModuleBase<SocketCommandContext>
        {

            [Command("technique")]
            [Alias("t")]
            public async Task UpdateTechnique([Summary("\nThe alias or name of the technique to update.")] string nameOrAlias,
                                              [Summary("\nkey=value pairs containing more informations about the technique.\n" +
                                                       "If a value contains more than one word, put it between \"\".\n" +
                                                       "To learn more about the available fields, use the command **data fields technique**.")]
                                              [Remainder] CommandOptions<Character> args)
            {
                try
                {
                    using var ctx = new EmeraldBotContext();
                    var t = ctx.Techniques.SingleOrDefault(x => (x.Name.Equals(nameOrAlias) || x.Alias.Equals(nameOrAlias))
                                                             && x.Server == ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id));
                    if (t == null) throw new Exception($"Couldn't find a technique that you can modify with alias or name = '{nameOrAlias}'");

                    t.Update(ctx, args.Params);
                    await ReplyAsync($"Technique '{t.Name}' has been successfully updated.");
                    ctx.SaveChanges();
                } catch (Exception e)
                {
                    await ReplyAsync($"Couldn't update '{nameOrAlias}': {e.Message}");
                    Console.WriteLine($"Exception: {e.Message}:\n{e.StackTrace}");
                }
            }

            [Command("advantage")]
            [Alias("a")]
            public async Task UpdateAdvantage([Summary("\nThe alias or name of the advantage to update.")] string nameOrAlias,
                                              [Summary("\nkey=value pairs containing more informations about the advantage.\n" +
                                                       "If a value contains more than one word, put it between \"\".\n" +
                                                       "To learn more about the available fields, use the command **data fields advantage**.")]
                                              [Remainder] CommandOptions<Character> args)
            {
                try
                {
                    using var ctx = new EmeraldBotContext();
                    var a = ctx.Advantages.SingleOrDefault(x => (x.Name.Equals(nameOrAlias) || x.Alias.Equals(nameOrAlias))
                                                            && x.Server == ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id));
                    if (a == null) throw new Exception($"Couldn't find an advantage with alias or name = '{nameOrAlias}'");

                    a.Update(ctx, args.Params);
                    await ReplyAsync($"Advantage '{a.Name}' has been successfully updated.");
                    ctx.SaveChanges();
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Couldn't update '{nameOrAlias}': {e.Message}");
                    Console.WriteLine($"Exception: {e.Message}:\n{e.StackTrace}");
                }
            }

            [Command("npc")]
            public async Task UpdateNPC([Summary("\nThe alias or name of the NPC to update.")] string nameOrAlias,
                                              [Summary("\nkey=value pairs containing more informations about the NPC.\n" +
                                                       "If a value contains more than one word, put it between \"\".\n" +
                                                       "To learn more about the available fields, use the command **data fields NPC**.")]
                                              [Remainder] CommandOptions<Character> args)
            {
                try
                {
                    using var ctx = new EmeraldBotContext();
                    var t = ctx.NPCs.SingleOrDefault(x => (x.Name.Equals(nameOrAlias) || x.Alias.Equals(nameOrAlias))
                                                            && x.Server == ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id));
                    if (t == null) throw new Exception($"Couldn't find an NPC with alias or name = '{nameOrAlias}'");

                    t.Update(ctx, args.Params);
                    ctx.SaveChanges();
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Couldn't update '{nameOrAlias}': {e.Message}");
                    Console.WriteLine($"Exception: {e.Message}:\n{e.StackTrace}");
                }
            }
        }
    }
}
