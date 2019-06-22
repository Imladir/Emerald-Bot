using Discord;
using Discord.Commands;
using EmeraldBot.Bot.Displays;
using EmeraldBot.Bot.Tools;
using EmeraldBot.Model;
using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Game;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Modules
{
    public class PrintAndList : ModuleBase<SocketCommandContext>
    {
        [Command("list")]
        [Alias("l")]
        [Summary("Displays a list of characters, techniques or skills")]
        public async Task PrintList([Summary("The first required parameter is the type of things you want to display. " +
                                             "It must be one of MyCharacters, Characters, AllCharacters, Skills or Techniques.")]
                                    string type,
                                    [Remainder]
                                    [Summary("You can filter those results by adding something after that, for example: " +
                                             "**!list techniques soul**.\n\n" +
                                             "Note that in the case of techniques, the filter is **required** (too many things to display at once otherwise).")]
                                    string filter = "")
        {
            filter = filter.ToLower().Trim();
            using var ctx = new EmeraldBotContext();
            try
            {
                List<string> msg;

                type = type.ToLower();
                switch (type)
                {
                    case "mycharacters": msg = PrintListCharacters(filter, 0); break;
                    case "characters": msg = PrintListCharacters(filter, 1); break;
                    case "allcharacters": msg = PrintListCharacters(filter, 2); break;
                    case "skills": msg = PrintListSkills(filter); break;
                    case "advantages": msg = PrintListAdvantages(filter); break;
                    case "techniques": msg = PrintListTechniques(filter); break;
                    default:
                        {
                            if (ctx.SkillGroups.SingleOrDefault(x => x.Name.Equals(type)) is SkillGroup sg)
                            {
                                msg = PrintListSkills(filter, sg);
                                break;
                            }
                            else if (ctx.TechniqueTypes.SingleOrDefault(x => x.Name.Equals(type)) is TechniqueType tt)
                            {
                                msg = PrintListTechniques(filter, tt);
                                break;
                            }
                            else if (ctx.AdvantageClasses.SingleOrDefault(x => x.Name.Equals(type)) is AdvantageClass ac)
                            {
                                msg = PrintListAdvantages(filter, ac);
                                break;
                            }
                            else
                            {
                                List<string> allowedKeys = new List<string> { "MyCharacters", "Characters", "Skills", "Techniques", "Advantages" };
                                ctx.Set<SkillGroup>().OrderBy(x => x.Name).ToList().ForEach(x => allowedKeys.Add(x.Name));
                                ctx.Set<TechniqueType>().OrderBy(x => x.Name).ToList().ForEach(x => allowedKeys.Add(x.Name));
                                ctx.Set<AdvantageClass>().OrderBy(x => x.Name).ToList().ForEach(x => allowedKeys.Add(x.Name));
                                throw new Exception($"Cannot list {type}. Allowed lists are {String.Join(" ", allowedKeys)}");
                            }
                        }
                }
                foreach (var m in msg.FitDiscordMessageSize()) await ReplyAsync(m);
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
                Console.WriteLine($"Exception: {e.Message}:\n{e.StackTrace}");
            }
        }

        public List<string> PrintListCharacters(string filter, int detailLevel)
        {
            List<string> msg = new List<string>();

            using (var ctx = new EmeraldBotContext())
            {
                IQueryable<PC> pcs = ctx.Characters.OfType<PC>().Where(x => x.Server.DiscordID == (long)Context.Guild.Id
                                                          && (filter.Equals("") || x.Name.Contains(filter) || x.Alias.Contains(filter)));

                if (detailLevel == 1) pcs = pcs.Where(x => x.Player.DefaultCharacters.Any(y => y.Character == x));
                else if (detailLevel == 0) pcs = pcs.Where(x => x.Player.DiscordID == (long)Context.User.Id);

                foreach (var c in pcs.OrderBy(x => x.Name))
                {
                    // Is it a default character?
                    bool def = ctx.Users.SingleOrDefault(x => x == c.Player
                                                             && x.DefaultCharacters.Any(y => y.Server.DiscordID == (long)Context.Guild.Id
                                                                                          && y.Player.ID == x.ID
                                                                                          && y.Character.ID == c.ID)) != null;
                    msg.Add(def ? $"**{c.Name}** (Alias: {c.Alias})" : $"{c.Name} (Alias: {c.Alias})");
                }
            }
            return msg;
        }

        public List<string> PrintListAdvantages(string filter, AdvantageClass ac = null)
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                List<string> msg = new List<string>();

                var advantages = ctx.Advantages.Include(x => x.AdvantageClass).Where(x => x.Name.Contains(filter) || x.Alias.Contains(filter));
                if (ac != null) advantages = advantages.Where(x => x.AdvantageClass == ac);
                if (advantages.Count() == 0) throw new Exception($"No {(ac == null ? "advantages" : ac.Name)} found with filter '{filter}'");

                foreach (var t in advantages.OrderBy(x => x.AdvantageClass.Name).ThenBy(x => x.Name))
                    msg.Add($"[{t.AdvantageClass.Name}] **{t.Name}** (alias: {t.Alias})");

                return msg;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed because of {e.Message}:\n{e.StackTrace}");
                throw new Exception($"Failed to print  {(ac == null ? "advantages" : ac.Name)} list: {e.Message}", e);
            }
        }

        public List<string> PrintListTechniques(string filter, TechniqueType tt = null)
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                if (tt == null && filter.Length < 2)
                    throw new Exception("There are too many techniques to list them all at once.\n" +
                                        "You need to either list them by type or add a filter of at least two characters");


                List<string> msg = new List<string>();

                var techniques = ctx.Techniques.Include(x => x.Type).Include(x => x.Ring)
                                               .Where(x => x.Name.Contains(filter) || x.Alias.Contains(filter));
                if (tt != null) techniques = techniques.Where(x => x.Type.ID == tt.ID);
                if (techniques.Count() == 0) throw new Exception($"No  {(tt == null ? "advantages" : tt.Name)} found with filter '{filter}'");

                foreach (var t in techniques.OrderBy(x => tt == null ? x.Type.Name : x.Ring.Name).ThenBy(x => x.Name))
                    msg.Add($"[{(tt == null ? t.Type.Name : t.Ring.Name)}] **{t.Name}** (alias: {t.Alias})");

                return msg;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed because of {e.Message}:\n{e.StackTrace}");
                throw new Exception($"Failed to print  {(tt == null ? "advantages" : tt.Name)} list: {e.Message}", e);
            }
        }

        public List<string> PrintListSkills(string filter, SkillGroup sg = null)
        {
            using var ctx = new EmeraldBotContext();
            try
            {
                List<string> msg = new List<string>();

                var skills = ctx.Skills.Include(x => x.Group).Where(x => x.Name.Contains(filter) || x.Alias.Contains(filter));
                if (sg != null) skills = skills.Where(x => x.Group == sg);
                if (skills.Count() == 0) throw new Exception($"No {(sg == null ? "" : sg.Name + " ")}skills found with filter '{filter}'");

                foreach (var t in skills.OrderBy(x => x.Group.Name).ThenBy(x => x.Name))
                    msg.Add($"[{t.Group.Name}] **{t.Name}** (alias: {t.Alias})");

                return msg;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed because of {e.Message}:\n{e.StackTrace}");
                throw new Exception($"Failed to print  {(sg == null ? "" : sg.Name + " ")}skills list: {e.Message}", e);
            }
        }

        [Command("print")]
        [Alias("p")]
        [Summary("Prints a character or technique")]
        public async Task Print([Summary("The alias of the character or technique to display.\n\n" +
                                         "If its a character you own and you're on your private channel, it will display everything, otherwise only the public profile.")]
                                string nameOrAlias,
                                string detailed = "")
        {
            ulong chanID = Context.Channel.Id;
            ulong serverID = Context.Guild.Id;
            var emds = new List<Embed>();

            using (var ctx = new EmeraldBotContext())
            {
                try
                {
                    var c = ctx.Characters.SingleOrDefault(x => x.Server.DiscordID == (long)serverID
                                                             && (x.Name.Equals(nameOrAlias) || x.Alias.Equals(nameOrAlias)));
                    if (c != null)
                    {
                        if (c is PC pc)
                            emds.AddRange(pc.GetProfile(Context.Guild.Id, Context.User.Id, Context.Channel.Id, detailed.Equals("full", StringComparison.OrdinalIgnoreCase)));
                        else if (c is NPC npc)
                            emds.AddRange(npc.GetProfile(Context.Guild.Id, Context.User.Id, Context.Channel.Id, detailed.Equals("full", StringComparison.OrdinalIgnoreCase)));
                    }
                    else
                    {
                        var na = ctx.GetNameAliasEntity(serverID, nameOrAlias);

                        if (na == null) throw new Exception("Nothing of that name has been found.");
                        else if (na is Advantage a)
                            emds.AddRange(a.GetProfile());
                        else if (na is Technique t)
                            emds.AddRange(t.GetProfile());
                    }

                    foreach (var e in emds) await ReplyAsync("", false, e);
                    await Context.Channel.DeleteMessageAsync(Context.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}: {e.StackTrace}");
                    var inner = e.InnerException;
                    while (inner != null)
                    {
                        Console.WriteLine($"Inner Exception:\n{inner.Message}: {inner.StackTrace}");
                        inner = inner.InnerException;
                    }
                }
            }
        }
    }
}
