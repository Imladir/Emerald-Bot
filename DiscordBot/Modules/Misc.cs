using Discord;
using Discord.Commands;
using EmeraldBot.Bot.Tools;
using EmeraldBot.Model;
using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Game;
using EmeraldBot.Model.Identity;
using EmeraldBot.Model.Rolls;
using EmeraldBot.Model.Servers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;

        public Misc(CommandService service) { _service = service; }

        [Command("help")]
        [Summary("Shows what a command does and the parameters it needs.")]
        public async Task Help([Remainder] string command = "")
        {

            char prefix = ' ';
            using (var ctx = new EmeraldBotContext())
            {
                prefix = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id).Prefix[0];
            }

            if (command == "")
            {
                var builder = new EmbedBuilder
                {
                    Color = new Color(114, 137, 218),
                    Description = "Commands available for the bot. To get more details about command *cmd*, type **!help cmd**"
                };

                foreach (var module in _service.Modules)
                {
                    string description = "";
                    foreach (var cmd in module.Commands)
                    {
                        var res = await cmd.CheckPreconditionsAsync(Context);
                        if (res.IsSuccess)
                            description += $"{prefix}{cmd.Aliases.First()}\n";
                    }

                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        builder.AddField(x =>
                        {
                            x.Name = module.Name;
                            x.Value = description;
                            x.IsInline = false;
                        });
                    }
                }
                await ReplyAsync("", false, builder.Build());
            }
            else
            {
                var result = _service.Search(Context, command);
                if (!result.IsSuccess)
                {
                    await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
                    return;
                }

                var builder = new EmbedBuilder
                {
                    Color = new Color(114, 137, 218),
                    Description = $"Help for command **{prefix}{command}**\n\nAliases: "
                };

                foreach (var match in result.Commands)
                {
                    var cmd = match.Command;
                    builder.AddField(x =>
                    {
                        x.Name = string.Join(", ", cmd.Aliases);
                        x.Value =
                            $"Summary: {cmd.Summary}\n\n" +
                            $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}: {string.Join("", cmd.Parameters.Select(p => p.Summary))}\n";
                        x.IsInline = false;
                    });
                }
                await ReplyAsync("", false, builder.Build());
            }
        }

        [RequireOwner]
        [Command("db")]
        public async Task ResetDB()
        {
            using var ctx = new EmeraldBotContext();
            using var dbTransaction = ctx.Database.BeginTransaction();
            try { 
                ctx.Seed();
                await ReplyAsync("Database has been seeded");

                var server = ctx.Servers.Single(x => x.DiscordID == 0);

                var rings = new Dictionary<int, string>()
                {
                    { 0, "Any" },
                    { 3, "Air" },
                    { 1, "Earth" },
                    { 2, "Fire" },
                    { 4, "Water" },
                    { 5, "Void" },
                };

                var skillGroups = new Dictionary<int, string>()
                {
                    { 1, "Artisan" },
                    { 2, "Martial" },
                    { 3, "Social" },
                    { 4, "Scholar" },
                    { 5, "Trade" }
                };


                string path = @"C:\Users\imlad\source\repos\GameData";

                // Skills
                var skillDef = new[] { new { Alias = "", Name = "", Group = 0 } };
                var skills = JsonConvert.DeserializeAnonymousType(File.ReadAllText(Path.Combine(path, "skills.json")), skillDef);
                var newSkills = new List<Skill>();
                foreach (var s in skills.OrderBy(x => x.Name))
                {
                    newSkills.Add(new Skill()
                    {
                        Alias = s.Alias,
                        Name = s.Name,
                        Server = server,
                        Source = new Source() { Book = "Core Rulebook", Page = -1 },
                        Group = ctx.SkillGroups.Single(x => x.Name.Equals(skillGroups[s.Group]))
                    });
                    Console.WriteLine($"Read skill {newSkills.Last().Name} from group {newSkills.Last().Group.Name}");
                }
                ctx.Skills.AddRange(newSkills);
                ctx.SaveChanges();
                await ReplyAsync("Skills are in.");

                // Advantages
                var advClass = new Dictionary<int, string>()
                {
                    { 1, "Distinction" },
                    { 2, "Adversity" },
                    { 3, "Passion" },
                    { 4, "Anxiety" }
                };

                var advTypes = new Dictionary<int, string>() {
                    { 1, "Curse" },
                    { 2, "Fame" },
                    { 4, "Flaw" },
                    { 8, "Infamy" },
                    { 16, "Interpersonal" },
                    { 32, "Mental" },
                    { 64, "Physical" },
                    { 128, "Scar" },
                    { 256, "Spiritual" },
                    { 512, "Virtue" }
                };

                var advDef = new[] { new { Class = 0, Alias = "", Type = 0, Name = "", PermanentEffect = "", RollEffect = "", Ring = 0 } };
                var advantages = JsonConvert.DeserializeAnonymousType(File.ReadAllText(Path.Combine(path, "advantages.json")), advDef);
                var newAdvantages = new List<Advantage>();
                foreach (var a in advantages.OrderBy(x => x.Name))
                {
                    var newA = new Advantage()
                    {
                        AdvantageClass = ctx.AdvantageClasses.Single(x => x.Name.Equals(advClass[a.Class])),
                        Alias = a.Alias,
                        Name = a.Name,
                        PermanentEffect = a.PermanentEffect,
                        Ring = ctx.Rings.Single(x => x.Name.Equals(rings[a.Ring])),
                        RollEffect = a.RollEffect,
                        Server = server,
                        Source = new Source() { Book = "Core Rulebook", Page = -1 },
                        AdvantageTypes = new List<AdvantageTypeAdvantages>()
                    };
                    foreach (var kv in advTypes)
                    {
                        if ((a.Type & kv.Key) == kv.Key)
                            newA.AdvantageTypes.Add(new AdvantageTypeAdvantages() { Advantage = newA, AdvantageType = ctx.AdvantageTypes.Single(x => x.Name.Equals(kv.Value)) });
                    }
                    newAdvantages.Add(newA);
                    Console.WriteLine($"Advantage {newA.Name} of class {newA.AdvantageClass.Name} has types {string.Join(", ", newA.AdvantageTypes.Select(x => x.AdvantageType.Name))}");
                }
                ctx.Advantages.AddRange(newAdvantages);
                ctx.SaveChanges();
                await ReplyAsync("Advantages are in.");

                // Techniques
                var techTypes = new Dictionary<int, string>()
                {
	                { 1, "Kata" },
	                { 2, "Kiho" },
	                { 3, "Invocation" },
	                { 4, "Ritual" },
	                { 5, "Shuji" },
	                { 6, "Maho" },
	                { 7, "Ninjutsu" }
                };

                var tecDef = new[] { new { Rank = 0, Alias = "", Type = 0, Name = "", Activation = "", Effect = "", Ring = 0, Skills = "", TN = 0, Source = "" } };
                var techniques = JsonConvert.DeserializeAnonymousType(File.ReadAllText(Path.Combine(path, "techniques.json")), tecDef);
                var newTechniques = new List<Technique>();
                foreach (var t in techniques.OrderBy(x => x.Name))
                {
                    var source = Regex.Split(t.Source, ", p");
                    var newT = new Technique() {
                        Activation = t.Activation,
                        Alias = t.Alias,
                        Effect = t.Effect,
                        Name = t.Name,
                        Rank = t.Rank,
                        Ring = ctx.Rings.Single(x => x.Name.Equals(rings[t.Ring])),
                        Source = new Source() { Book = source[0], Page = int.Parse(source[1]) },
                        TN = t.TN,
                        Type = ctx.TechniqueTypes.Single(x => x.Name.Equals(techTypes[t.Type])),
                        Skills = new List<TechniqueSkill>(),
                        SkillGroups = new List<TechniqueSkillGroup>(),
                        Server = server
                    };

                    foreach (var s in Regex.Split(t.Skills, "[,;]"))
                    {
                        if (string.IsNullOrWhiteSpace(s)) continue;
                        var skill = ctx.Skills.SingleOrDefault(x => x.Alias.Equals(s));
                        if (skill != null) newT.Skills.Add(new TechniqueSkill() { Skill = skill, Technique = newT });
                        else newT.SkillGroups.Add(new TechniqueSkillGroup() { SkillGroup = ctx.SkillGroups.Single(x => x.Name.Equals(s)), Technique = newT });
                    }
                    newTechniques.Add(newT);
                    Console.WriteLine($"Technique {newT.Name} of type {newT.Type.Name} is associated to {newT.Ring.Name} and {newT.Skills.Count} skills");
                }
                ctx.Techniques.AddRange(newTechniques);
                ctx.SaveChanges();
                await ReplyAsync("Techniques are in.");

                // Players
                server = new Server()
                {
                    DiscordID = 577923638422929408,
                    DiceChannelID = 577923736817106956
                };
                var defaultCharacter = new Dictionary<Guid, long>();
                var playersPath = Path.Combine(path, "Players");
                var playerDef = new { Identity = new { ServerID = (long)0, UserID = (long)0 }, Username = "", DefaultCharacter = new Guid(), PrivateChannelID = (long)0 };
                var players = new List<User>();
                foreach (String f in Directory.EnumerateFiles(playersPath))
                {
                    var p = JsonConvert.DeserializeAnonymousType(File.ReadAllText(f), playerDef);
                    var newP = new User()
                    {
                        UserName = p.Username,
                        DiscordID = p.Identity.UserID,
                        LastUpdate = DateTime.UtcNow,
                        PrivateChannels = new List<PrivateChannel>(),
                        Roles = new List<UserRole>()
                    };
                    newP.PrivateChannels.Add(new PrivateChannel() { Player = newP, ChannelDiscordID = p.PrivateChannelID, Server = server});
                    defaultCharacter[p.DefaultCharacter] = newP.DiscordID;

                    if (newP.DiscordID == 306611724113412106)
                    {
                        newP.Roles.Add(new UserRole() { User = newP, Role = ctx.Roles.Single(x => x.Name.Equals("Admin")), Server = server });
                        newP.Roles.Add(new UserRole() { User = newP, Role = ctx.Roles.Single(x => x.Name.Equals("GM")), Server = server });
                    }
                    else if (newP.DiscordID == 158005236550598656)
                        newP.Roles.Add(new UserRole() { User = newP, Role = ctx.Roles.Single(x => x.Name.Equals("ServerOwner")), Server = server });
                    else if (newP.DiscordID == 374387705368281089)
                        newP.Roles.Add(new UserRole() { User = newP, Role = ctx.Roles.Single(x => x.Name.Equals("GM")), Server = server });
                    Console.WriteLine($"Finished reading player {newP.UserName}");
                    players.Add(newP);
                }
                ctx.Users.AddRange(players);
                ctx.SaveChanges();
                await ReplyAsync("Players are in.");

                // Characters
                var charactersPath = Path.Combine(path, "Characters");
                var characterDef = new {
                    UserID = (long)0,
                    CharacterID = new Guid(),
                    School = "",
                    Rank = 0,
                    Age = 0,
                    Ninjo = "",
                    Giri = "",
                    Techniques = new[] { "" },
                    Advantages = new[] { "" },
                    XP = new[] { new {
                        DateEntry = DateTime.Now,
                        Amount = 0,
                        Reason = ""
                    } },
                    Glory = new[] { new {
                        DateEntry = DateTime.Now,
                        Amount = 0,
                        Reason = ""
                    } },
                    Status = new[] { new {
                        DateEntry = DateTime.Now,
                        Amount = 0,
                        Reason = ""
                    } },
                    Honour = new[] { new {
                        DateEntry = DateTime.Now,
                        Amount = 0,
                        Reason = ""
                    } },
                    Name = "",
                    Alias = "",
                    Clan = "",
                    Family = "",
                    Description = "",
                    IconURL = "",
                    Rings = new Dictionary<string, int>(),
                    Skills = new Dictionary<string, int>()
                };
                var pcs = new List<PC>();
                foreach (String f in Directory.EnumerateFiles(charactersPath))
                {
                    var pc = JsonConvert.DeserializeAnonymousType(File.ReadAllText(f), characterDef);
                    var newPC = new PC()
                    {
                        Age = pc.Age,
                        Alias = pc.Alias,
                        Clan = ctx.Clans.Single(x => x.Name.Equals(pc.Clan)),
                        Name = pc.Name,
                        Description = pc.Description,
                        Family = pc.Family,
                        Giri = pc.Giri,
                        Icon = pc.IconURL,
                        Ninjo = pc.Ninjo,
                        Player = ctx.Users.Single(x => x.DiscordID == (long)pc.UserID),
                        Rank = pc.Rank,
                        School = pc.School,
                        Server = server
                    };
                    newPC.InitRings(ctx);
                    if (pc.Rings != null) foreach (var r in pc.Rings) newPC.Ring(r.Key, r.Value);
                    if (pc.Skills != null) foreach (var s in pc.Skills) newPC.UpdateSkill(ctx, s.Key, s.Value);
                    if (pc.Advantages != null) foreach (var a in pc.Advantages) newPC.AddAdvantage(ctx, a);
                    if (pc.Techniques != null) foreach (var t in pc.Techniques) newPC.AddTechnique(ctx, t);
                    newPC.ResetVoid();
                    newPC.UpdateSecondaryStats();

                    if (pc.XP != null)
                    foreach (var je in pc.XP)
                        newPC.JournalEntries.Add(new JournalEntry()
                        {
                            Amount = je.Amount,
                            EntryDate = je.DateEntry,
                            Journal = ctx.JournalTypes.Single(x => x.Name.Equals("XP")),
                            Reason = je.Reason
                        });

                    if (pc.Glory != null)
                        foreach (var je in pc.Glory)
                        newPC.JournalEntries.Add(new JournalEntry()
                        {
                            Amount = je.Amount,
                            EntryDate = je.DateEntry,
                            Journal = ctx.JournalTypes.Single(x => x.Name.Equals("Glory")),
                            Reason = je.Reason
                        });

                    if (pc.Honour != null)
                        foreach (var je in pc.Honour)
                        newPC.JournalEntries.Add(new JournalEntry()
                        {
                            Amount = je.Amount,
                            EntryDate = je.DateEntry,
                            Journal = ctx.JournalTypes.Single(x => x.Name.Equals("Honour")),
                            Reason = je.Reason
                        });

                    if (pc.Status != null)
                        foreach (var je in pc.Status)
                        newPC.JournalEntries.Add(new JournalEntry()
                        {
                            Amount = je.Amount,
                            EntryDate = je.DateEntry,
                            Journal = ctx.JournalTypes.Single(x => x.Name.Equals("Status")),
                            Reason = je.Reason
                        });
                    if (defaultCharacter.ContainsKey(pc.CharacterID))
                    {
                        var user = ctx.Users.Single(x => x.DiscordID == pc.UserID);
                        user.DefaultCharacters = new List<DefaultCharacter>();
                        user.DefaultCharacters.Add(new DefaultCharacter() { Character = newPC, Player = user, Server = server });
                    }
                    Console.WriteLine($"Dealt with character {pc.Name} - {pc.Alias}");
                    pcs.Add(newPC);
                }
                ctx.PCs.AddRange(pcs.OrderByDescending(x => x.School));
                ctx.SaveChanges();
                await ReplyAsync("Characters are done.");


                dbTransaction.Commit();
                await ReplyAsync("Everyhting is saved.");
            }
            catch (Exception e)
            {
                dbTransaction.Rollback();
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        [Command("test")]
        public async Task Test()
        {
            using var ctx = new EmeraldBotContext();
            string s = $"Possible rolls on a a Ring die: ";
            var defs = ctx.DieFaces.Where(x => x.DieType == "Ring").OrderBy(x => x.Value);
            defs.ToList().ForEach(x => s += x.Emote.Text);
            s += $"\nPossible rolls on a a Skill die: ";
            defs = ctx.DieFaces.Where(x => x.DieType == "Skill").OrderBy(x => x.Value);
            defs.ToList().ForEach(x => s += x.Emote.Text);
            await ReplyAsync(s);
        }
    }
}
