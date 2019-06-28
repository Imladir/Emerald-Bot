using Discord;
using Discord.Commands;
using EmeraldBot.Bot.Tools;
using EmeraldBot.Model;
using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Game;
using EmeraldBot.Model.Identity;
using EmeraldBot.Model.Rolls;
using EmeraldBot.Model.Servers;
using Microsoft.AspNetCore.Identity;
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



                /////////////////////////////
                //         Skills          //
                /////////////////////////////
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



                /////////////////////////////
                //      Advantages         //
                /////////////////////////////
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



                /////////////////////////////
                //       Techniques        //
                /////////////////////////////
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

                /////////////////////////////
                //        Players          //
                /////////////////////////////
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
                        newP.Claims.Add(new UserClaim() { ClaimType = $"Server-{server.DiscordID}", ClaimValue = "GM" });
                    }
                    else if (newP.DiscordID == 158005236550598656)
                    {
                        newP.Roles.Add(new UserRole() { User = newP, Role = ctx.Roles.Single(x => x.Name.Equals("ServerOwner")), Server = server });
                        newP.Claims.Add(new UserClaim() { ClaimType = $"Server-{server.DiscordID}", ClaimValue = "ServerOwner" });
                    }
                    else if (newP.DiscordID == 374387705368281089)
                    {
                        newP.Roles.Add(new UserRole() { User = newP, Role = ctx.Roles.Single(x => x.Name.Equals("GM")), Server = server });
                        newP.Claims.Add(new UserClaim() { ClaimType = $"Server-{server.DiscordID}", ClaimValue = "GM" });
                    }
                    Console.WriteLine($"Finished reading player {newP.UserName}");
                    players.Add(newP);
                }
                ctx.Users.AddRange(players);
                ctx.SaveChanges();
                await ReplyAsync("Players are in.");



                /////////////////////////////
                //      Characters         //
                /////////////////////////////
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

        [RequireOwner]
        [Command("gear")]
        public async Task Test()
        {
            using var ctx = new EmeraldBotContext();
            List<GearQuality> quals = new List<GearQuality>() {
                            new GearQuality() { Name = "Ceremonial", Description = @"Ceremonial equipment is indicative of the wearer’s rank. It can be brought into many contexts in whichsimilar items might not be allowed. Rarely, a character might be loaned a Ceremonial item to provide them with authority during a task.
            While wearing one or more Ceremonial items openly, reduce TN of checks to convince others of your real or assumed identity as the known owner of that item (or one of their allies) by 1.
            At the end of any scene in which you use a Ceremonial item of a character with higher status without their permission, you must forfeit 3 honor.", Source = new Source() { Book = "Core Rulebook", Page = 240 }},
                            new GearQuality() { Name = "Concealable", Description = @"Some weapons and armor are easy to hide on one’s person, due to size or deliberate design.
            Unless explicitly worn openly or revealed for use, a Concealable item is assumed to be hidden. When observing the wearer of a hidden item, a character must succeed at a **TN 3 Design (Air)** or **Smithing (Air)** check to discern that the wearer is armed or armored. If the onlooker succeeds with two or more bonus successes, they also determine the type of the hidden item.
            Concealable armor can be worn under loose-fitting clothes, but you can only benefit from the resistance and qualities of one set of armor at a time (see page 238).
            Concealable weapons can also be drawn more quickly in combat. As part of an Attack action, you may ready or sheathe one Concealable weapon.", Source = new Source() { Book = "Core Rulebook", Page = 240 }},
                            new GearQuality() { Name = "Cumbersome", Description = @"This item is heavy or unwieldy, and impossible to conceal on one’s person.
            While wearing Cumbersome armor, increase the TN of your checks to maneuver in your environment (such as Movement action checks) by 1.
            If you moved this turn, increase the TN of Attack action checks using a Cumbersome weapon by 1.", Source = new Source() { Book = "Core Rulebook", Page = 240 }},
                            new GearQuality() { Name = "Damaged", Description = @"Samurai must carefully maintain their kit in the field, for routine wear and tear will eventually ruin nearly any tool.
            While many samurai have staff to assist with such matters, most battle-tested samurai have learned at least the basics needed to keep their equipment functional.
            If a weapon or tool becomes Damaged, increase the TN of checks to use it by 1.
            If armor becomes Damaged, reduce all resistances it provides by 2 (to a minimum of 0).
            If a Damaged item becomes Damaged again, it loses Damaged and becomes Destroyed instead. If it becomes Destroyed for any other reason, it also loses Damaged.", Source = new Source() { Book = "Core Rulebook", Page = 240 }},
                            new GearQuality() { Name = "Destroyed", Description = @"Extreme events or long use without maintenance can shatter, tear, or otherwise render an item nonfunctional.
            Such an item is broken and cannot be used for its intended function. It might still be usable as an improvised weapon or tool, at the GM’s discretion.", Source = new Source() { Book = "Core Rulebook", Page = 240 }},
                            new GearQuality() { Name = "Durable", Description = @"Items crafted to last can endure damage that would destroy less well-made works.
            If a Durable item would become Damaged, it loses Durable instead. If a Durable item would be Destroyed, it loses Durable and becomes Damaged instead.", Source = new Source() { Book = "Core Rulebook", Page = 240 }},
                            new GearQuality() { Name = "Forbidden", Description = @"Rokugani laws are unforgiving, and many things are contraband. This includes many illicit substances, heretical art or literature, and foreign items that have not been transported through strictly regulated trade routes.
            At the end of any scene in which you used a Forbidden item openly in public, you must forfeit 3 glory.", Source = new Source() { Book = "Core Rulebook", Page = 240 }},
                            new GearQuality() { Name = "Mundane", Description = @"Mundane items are objects that are found in all contexts of daily life, and are permitted to anyone in most social contexts (within reason). After all, people in many walks of life need tools like knives, walking sticks, and wheat threshers, even if these items can be utilized to deadly effect by someone sufficiently determined.
            Wearing a Mundane item openly has no additional effects, though onlookers still take note of the fact that you are armed if the item could be used as a weapon.", Source = new Source() { Book = "Core Rulebook", Page = 240 }},
                            new GearQuality() { Name = "Prepare", Description = @"This item must be reloaded, reset, or otherwise prepared after use.
            After you use this item, you cannot use it again until it has been **prepared**. As a Support action, while it is readied, you may prepare this item for use.", Source = new Source() { Book = "Core Rulebook", Page = 240 }},
                            new GearQuality() { Name = "Razor-Edged", Description = @"Some weapons are particularly sharp, and thus brittle, and must be carefully honed to keep functioning at their best. The traditional Rokugani katana is one such weapon—its keen edge lets it hew off limbs more easily than other swords, but it is also more susceptible to damage if swung carelessly at armored foes. When you succeed at an Attack action check that deals damage to a target using a Razor-Edged item, if the damage dealt is reduced to 0 before they defend, this item becomes Damaged.
            When you make an Attack action check with a Razor-Edged weapon, you may spend {opportunity} as follows:
            {opportunity}+: Treat the deadliness of this weapon as 1 higher per {opportunity} spent this way.", Source = new Source() { Book = "Core Rulebook", Page = 241 }},
                            new GearQuality() { Name = "Resplendent", Description = @"An item with the Resplendent quality is embellished to attract attention to its presenter or wearer.
            While wearing one or more Resplendent items, if you would be awarded 1 or more glory, you receive that amount of glory plus 1 instead.", Source = new Source() { Book = "Core Rulebook", Page = 241 }},
                            new GearQuality() { Name = "Sacred", Description = @"Items with the Sacred quality are imbued with purifying power—often in the form of jade, the mystical stone said to be the tears shed by Amaterasu, the goddess of the sun and protector against the terror of the night. Such items are anathema to the denizens of the Shadowlands and other horrors that prey upon humanity.
            When an Otherworldly being makes an Attack or Intrigue action check targeting the wearer of one or more Sacred items, increase the TN by 1.
            While you are carrying one or more Sacred items, if you would gain the Afflicted condition, you must choose one of those items to become Damaged instead.
            Sacred weapons ignore all resistances possessed by Otherworldly and Tainted beings.", Source = new Source() { Book = "Core Rulebook", Page = 241 }},
                            new GearQuality() { Name = "Snaring", Description = @"Flexible or, in some cases, harshly curved weapons are excellent for snaring, grabbing, and disarming. When performing an Attack action with a Snaring weapon, you may spend {opportunity} as follows:
            {opportunity}+: One target of the attack with vigilance lower than or equal to the {opportunity} spent this way suffers the Immobilized condition.", Source = new Source() { Book = "Core Rulebook", Page = 241 }},
                            new GearQuality() { Name = "Subtle", Description = @"An item with the Subtle quality is made to not stand out, and people tend to overlook it.
            Increase the TN of checks to ascertain information about the item by 1. At the GM’s discretion, this also applies to checks to discern what the creator or wearer of the piece intends by presenting it, or to learn anything of value about that person from it.", Source = new Source() { Book = "Core Rulebook", Page = 241 }},
                            new GearQuality() { Name = "Unholy", Description = @"Some physical artifacts carry a dreadful miasma. They may have been tainted by direct exposure to the power of the Shadowlands or by foul rituals, but some are made with obsidian, said to be the shed blood of Onnotangu, the god of the moon and ancient foe of the Kami.
            After a character suffers a critical strike from an Unholy weapon, they suffer the Afflicted condition.
            At the end of each scene in which you use an Unholy item, you suffer the Afflicted condition. Unholy weapons ignore all resistances possessed by Otherworldly beings.", Source = new Source() { Book = "Core Rulebook", Page = 241 }},
                            new GearQuality() { Name = "Wargear", Description = @"Implements seen in battle and few other contexts - items with the Wargear quality are decidedly inappropriate for most social contexts. Wargear makes people nervous, which impedes efforts at diplomacy.
            Whenever another character suffers 1 or more strife due to the actions of someone wearing one or more Wargear items, they suffer that amount plus 1 instead.", Source = new Source() { Book = "Core Rulebook", Page = 241}},
                        };
            ctx.GearQualities.AddRange(quals);
            ctx.SaveChanges();

            var dataServer = ctx.Servers.Single(x => x.DiscordID == 0);
            var gear = new List<Gear>()
            {
                new Gear() { Alias = "bottleSake", Name = "Bottle of Sake", Description = @"Sake, a wine made from fermented rice, is popular throughout Rokugan and is readily available at any inn. The quality of sake varies widely depending on the maker and the seller.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 242 }, Cost = new MoneySum() { Koku = 0, Bu = 1, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 1, MaxRarity = 1 } },
                new Gear() { Alias = "bowyerKit", Name = "Bowyer’s Kit", Description = @"A bowyer’s kit is a small tool kit used to maintain bows and arrows. It typically includes a selection of small hand tools, spare string, some feathers, wax, bamboo fibers, arrowheads, and other items needed to keep a bow and arrows in working order.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 242 }, Cost = new MoneySum() { Koku = 0, Bu = 2, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 1, MaxRarity = 1 } },
                new Gear() { Alias = "calligraphySet", Name = "Calligraphy Set", Description = @"These ornate boxes contain items a practiced calligrapher needs to craft books, letters, scrolls, and other written communications. It consists of a small wooden box that contains a variety of brushes, inkstones, and pigments; a small bowl for water; and a small bag of sand for drying. Calligraphers and scribes customarily carry a calligraphy set, as so do some samurai and courtiers who prefer to write their own correspondence.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 242 }, Cost = new MoneySum() { Koku = 0, Bu = 1, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 3, MaxRarity = 3 } },
                new Gear() { Alias = "chopsticks", Name = "Chopsticks", Description = @"Chopsticks are the primary Rokugani eating utensils.
Sold in pairs, these slender utensils are normally made of wood or bamboo, although some wealthy individuals use chopsticks of ivory, bone, or metal instead.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 242 }, Cost = new MoneySum() { Koku = 0, Bu = 0, Zeni = 1 }, Rarity = new GearRarity() { MinRarity = 1, MaxRarity = 1 } },
                new Gear() { Alias = "daishoStand", Name = "Daishō Stand", Description = @"This is a small, collapsible wooden stand used to display a samurai’s daishō. There is a particular etiquette to displaying the daishō on a rack that speaks to an individual’s ease or readiness to fight. If the swords are displayed with their pommels facing right, this means that the individual is ready to fight, as they can easily draw the swords from the rack with their right hand. If they are displayed with the pommels facing left, this means that the individual is less ready to fight but still guarded. If the pommels are facing left and the tsuba are on the inside of the rack arm, making drawing the swords impossible in one motion, this means that the individual is completely at ease.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 242 }, Cost = new MoneySum() { Koku = 1, Bu = 0, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 4, MaxRarity = 4 } },
                new Gear() { Alias = "diceAndCup", Name = "Dice and Cup", Description = @"These are used to play a variety of dice games throughout Rokugan. Gambling is extremely popular in the Emerald Empire, but it is largely frowned upon by samurai. Organized gambling is controlled largely by various criminal syndicates and is often a source of corruption among local officials.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 242 }, Cost = new MoneySum() { Koku = 0, Bu = 0, Zeni = 25 }, Rarity = new GearRarity() { MinRarity = 2, MaxRarity = 2 } },
                new Gear() { Alias = "divinationKit", Name = "Divination Kit", Description = @"Popular among both superstitious peasants and the mystical shugenja, divination kits are used to tell the future or otherwise commune with the Spirit Realms. A divination kit may contain special coins, sticks, bones, or other small items with metaphysical import. Diviners use these kits by casting their coins or sticks on the ground and reading the patterns they create for any omens or portents.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 242 }, Cost = new MoneySum() { Koku = 0, Bu = 1, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 4, MaxRarity = 4 } },
                new Gear() { Alias = "fingerJade", Name = "Finger of Jade", Description = @"A finger of jade is a length of the precious green stone worn about the neck on a thong to ward off the corrupting effects of the Taint. Bit by bit, the finger of jade is consumed by this process.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 242 }, Cost = new MoneySum() { Koku = 1, Bu = 0, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 6, MaxRarity = 6 } },
                new Gear() { Alias = "gameSet", Name = "Games", Description = @"These are small, portable, sturdy versions of games made to be taken on long journeys. Go, shōgi, and other games popular among bushi and courtiers are the ones most likely to be found in a travel-sized set, but there are as many different types of travel games as there are full-sized versions.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 242 }, Cost = new MoneySum() { Koku = 0, Bu = 1, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 3, MaxRarity = 3 } },
                new Gear() { Alias = "kubiBukuro", Name = "Kubi Bukuro", Description = @"These are simple net bags used to carry the severed head of an enemy, which is considered both a trophy and a good omen. Lion Clan samurai use them to carry their trophies without touching dead flesh, but the Kuni family uses them for collecting samples of Shadowlands creatures for study.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 242 }, Cost = new MoneySum() { Koku = 0, Bu = 0, Zeni = 2 }, Rarity = new GearRarity() { MinRarity = 2, MaxRarity = 2 } },
                new Gear() { Alias = "luckyCricket", Name = "Lucky Cricket", Description = @"Very popular among the Mantis Clan, a small live crick- et in an ornate metal cage is thought to bring good fortune to whomever carries it.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 242 }, Cost = new MoneySum() { Koku = 0, Bu = 4, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 4, MaxRarity = 4 } },
                new Gear() { Alias = "medicineKit", Name = "Medicine Kit", Description = @"This is a simple first aid kit that contains all the nec- essary items to treat many kinds of injuries. A typical kit contains needle and thread, cloth bandages, herbal disinfectants, various balms and tinctures, and other healing items.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 243 }, Cost = new MoneySum() { Koku = 0, Bu = 1, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 3, MaxRarity = 3 } },
                new Gear() { Alias = "musicalInstrument", Name = "Musical Instrument", Description = @"Rokugani musical traditions are rich and varied. Many people in the Empire can at least pick out a simple tune on a flute or drum. Instruments also include the shamisen, biwa, and koto. Many courtiers carry an instrument to display their talents, and drums are popular among the samurai class for signaling troops and sending messages.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 243 }, Cost = new MoneySum() { Koku = 0, Bu = 3, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 2, MaxRarity = 6 } },
                new Gear() { Alias = "omamori", Name = "Omamori", Description = @"Omamori are protective charms, sold at many shrines across the Emerald Empire, often in the form of a small envelope or bag containing a written or inscribed prayer to a particular kami (often one of the great Fortunes, but occasionally others). Most examples provide protection against ill fortune of a specific variety such as illness or accidents, or encourage good luck in some sphere, such as farming, travel, or marriage, but are never put to the test in any rigorous manner—the kami move by paths unseen by mortal eyes, after all, and it is hard to determine if a talisman was truly responsible for one’s safety.
Shugenja and other powerful servants of the kami are capable of creating especially efficacious protective charms. These frequently contain appeals to the seven Great Fortunes (such as the three listed here), but can also offer other protections by appealing to different powers.
An omamori might contain one of the following blessings:
$ **Boon of Fukurokujin**: Fukurokujin, Fortune of Wisdom, illuminates the truth even when tricksters seek to obscure it. Once per game session, the wearer may spend 1 Void point to remove a condition or persistent effect caused by a Scheme action.
$ **Boon of Bishamon**: Bishamon promises glory at arms to the bold, and draws the eyes of the powerful to those who prove their strength. Once per game session, when the wearer eceives a glory award, the wearer may spend 1 Void point to increase that award by 3.
$ **Boon of Benten**: Benten watches over artists and lovers, guiding them in their endeavors. Once per game session, after rolling dice, the wearer may spend 1 Void point to add a kept {r-} set to an {opportunity} result to an Artisan or Social check. 
The kami can be a bit jealous in their stewardship, however. While a character can theoretically wear multiple protective charms at once, they cannot benefit from any of these blessings while they are wearing more than one omamori.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 243 }, Cost = new MoneySum() { Koku = 0, Bu = 5, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 2, MaxRarity = 2 } },
                new Gear() { Alias = "personalSealChop", Name = "Personal Seal or Chop", Description = @"A personal chop is used by samurai to verify their iden- tity and sign documents, among other official business.
Each one is a unique design that is registered with the Miya family. Using another samurai’s chop is considered both an insult and a serious crime.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 243 }, Cost = new MoneySum() { Koku = 0, Bu = 4, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 6, MaxRarity = 6 } },
                new Gear() { Alias = "pillowBook", Name = "Pillow Book", Description = @"This is a small, loosely bound, cheap- ly made book written in a genre some samurai consider insufficiently serious, such as romance, adventure, or poetry. A pillow book can also be an account of a famous person’s life or an adaptation of a diary. Occasionally, important literary or academic works are converted to pillow books to make them more portable.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 243 }, Cost = new MoneySum() { Koku = 0, Bu = 0, Zeni = 3 }, Rarity = new GearRarity() { MinRarity = 2, MaxRarity = 2 } },
                new Gear() { Alias = "poisonVial", Name = "Poison (one vial)", Description = @"In the courts of the Emerald Empire, the wheels of pol- itics are sometimes turned by unsavory means. Members of the Scorpion Clan are certainly not above using poison to weaken or eliminate political rivals—not that one could ever prove their culpability—and many samurai turn to such measures in times of desperation or fear.
Three poisons found in Rokugan are:
$ Noxious Poison: A catchall term for a wide variety of deadly compounds (many of which have perfectly legitimate uses in medicine and other trades), these sorts of poisons often act by attacking the heart, organs, or brain directly.
– When a character ingests noxious poison, the character suffers a critical strike with severity 10. The poisoner may spend {oportunity} on their check to deliver the poison to increase the severity by 1 per {opportunity} spent this way.
– When noxious poison is applied to a weap- on, increase that weapon’s deadliness by +4.
$ Fire Biter: A poison that wracks victims with searing pain, fire biter is easily identified by its bitter taste, which makes it difficult to disguise its presence in food or drink.
– When a character ingests fire biter, the character suffers 5 strife and the Dazed and Exhausted conditions.
– When fire biter is applied to a weapon, after a target suffers a critical strike from the weapon, they suffer the Dazed and Exhausted conditions.
$ Night Milk: A favorite of the Shosuro family, night milk is a dizzying concoction that must be injected or otherwise delivered directly into the bloodstream.
– When night milk is introduced into a character’s bloodstream, the character suffers 5 fatigue and the Disoriented and Prone conditions.
– When night milk is applied to a weapon, after a target suffers a critical strike from the weapon, they suffer the Disoriented and Prone conditions.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 244 }, Cost = new MoneySum() { Koku = 0, Bu = 0, Zeni = 30 }, Rarity = new GearRarity() { MinRarity = 5, MaxRarity = 5 } },
                new Gear() { Alias = "quiverArrows", Name = "Quiver of Arrows", Description = @"A quiver is a cloth or leather container used to carry arrows. Worn at the waist or slung over the shoulder, a standard quiver can hold sixty or more arrows. There are also quivers or equivalent carrying pouches for crossbow quarrels, blowgun darts, stones, and other ammunition, and they function in the same manner.
As long as a character has a quiver of arrows available, they are assumed not to run out of standard ammunition. Further, the character is assumed to refill the quiver at reasonable opportunities, such as when visiting a guard outpost or a castle with an armory. 
If a character goes an especially long time without having a reasonable opportunity to refill their quiver, the GM may state that the quiver is running low, with only 3 standard pieces of ammunition remaining; the character then gains 1 Void point. The character still possesses any special arrows they had prior to this point.
A depleted quiver can be refilled by visiting a location where the character can acquire ammunition, or by making ammunition using the Survival skill if the character has the proper supplies (see page 169).", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 244 }, Cost = new MoneySum() { Koku = 0, Bu = 0, Zeni = 20 }, Rarity = new GearRarity() { MinRarity = 1, MaxRarity = 1 } },
                new Gear() { Alias = "rope", Name = "Rope (By the foot)", Description = @"Rope is made from a variety of materials and is used for tasks as varied as binding enemies, restraining livestock, climbing, and rigging sailing vessels. Low-quality ropes are made of hemp and tend to be both rough and stiff. Higher-quality ropes are made of hair, silk, or more exotic fibers.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 244 }, Cost = new MoneySum() { Koku = 0, Bu = 0, Zeni = 15 }, Rarity = new GearRarity() { MinRarity = 1, MaxRarity = 1 } },
                new Gear() { Alias = "sakeCup", Name = "Sake Cup", Description = @"These are small wood, porcelain, or metal cups used for drinking sake. In many areas of the Empire, it is customary for a guest to bring their own cup to gatherings and official functions. Wealthy or influential people tend to carry ornately decorated cups made from expensive materials to better impress fellow drinkers.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 245 }, Cost = new MoneySum() { Koku = 0, Bu = 1, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 2, MaxRarity = 2 } },
                new Gear() { Alias = "spices", Name = "Spices", Description = @"Spices are important both for flavoring food and as a trade commodity throughout Rokugan. While some important varieties are native to the Emerald Empire, such as wasabi and sanshō pepper, many are imported from abroad. Depending on their provenance and rarity, spices can be extremely expensive. Much blood has been spilled by various factions in attempts to control the spice trade.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 245 }, Cost = new MoneySum() { Koku = 0, Bu = 3, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 5, MaxRarity = 7 } },
                new Gear() { Alias = "sweets", Name = "Sweets (Four Servings)", Description = @"These are candies, small cakes, and other confections that are popular during the Empire’s many festival seasons. Typical Rokugani sweets are made from bean curd or honeyed rice, but a few gaijin treats made from exotic and foreign ingredients are produced here and there throughout Rokugan. This is especially true in the Unicorn lands.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 245 }, Cost = new MoneySum() { Koku = 0, Bu = 1, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 1, MaxRarity = 1 } },
                new Gear() { Alias = "tattooNeedles", Name = "Tattoo Needles", Description = @"Extremely popular among the Dragon Clan, these are steel or bamboo needles used to apply tattoos. Tattoo needles are ordinarily part of a kit that includes needles, various pigments, and other tools used by tattoo artists in their work. Widespread not only in the Dragon Clan, tattoos are also prevalent among the Empire’s numerous criminal cartels, which use them both to identify members of specific cartels and to record individuals’ criminal exploits.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 245 }, Cost = new MoneySum() { Koku = 0, Bu = 1, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 4, MaxRarity = 4 } },
                new Gear() { Alias = "teaSet", Name = "Tea Set (Portable)", Description = @"The tea ceremony is exceptionally important in Rokugani society. These small, portable tea sets are designed to withstand rough handling and travel. They allow samurai to enjoy the harmony and relaxation of the tea ceremony even when they are far from home.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 245 }, Cost = new MoneySum() { Koku = 1, Bu = 0, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 6, MaxRarity = 6 } },
                new Gear() { Alias = "tentChomchong", Name = "Tent (Chomchong)", Description = @"Chomchong are large, elaborate, portable homes popular with the Unicorn Clan. Sturdy and well protected from the elements, these tents can accommodate up to a dozen people in relative comfort. They are also used to house small noble families in as much luxury as can be afforded in the field. Chomchong take quite a bit of time to set up and tear down, and carrying them requires several oxen and a large wagon.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 245 }, Cost = new MoneySum() { Koku = 20, Bu = 0, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 7, MaxRarity = 7 } },
                new Gear() { Alias = "tentSmall", Name = "Tent (Small)", Description = @"These basic shelters are just big enough to keep two individuals warm and dry in the wilderness. They are extremely easy to set up and take down, pack down relatively small, and can be carried on a person’s back, in a cart, or slung over a horse.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 245 }, Cost = new MoneySum() { Koku = 1, Bu = 0, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 3, MaxRarity = 3 } },
                new Gear() { Alias = "tentYurt", Name = "Tent (Yurt)", Description = @"Bigger than basic tents but not as big or luxurious as chomchong, yurts were brought to Rokugan from the far-off steppes by the Unicorn Clan. Made from thick leather or canvas with a felt floor, a yurt can house a group of four indefinitely in relative comfort. They are heavy and awkward to build or take down, and require either a horse or oxen dedicated to carting them around.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 245 }, Cost = new MoneySum() { Koku = 10, Bu = 0, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 5, MaxRarity = 5 } },
                new Gear() { Alias = "rations", Name = "Travelling Rations", Description = @"Traveling rations consist of a mix of preserved foods sufficient to keep an individual fed and healthy while on the road. They typically consist of a mix of dried fish, rice balls wrapped in paper, water or tea, pickled vegetables, and other hardy foods that can withstand exposure to the elements.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 245 }, Cost = new MoneySum() { Koku = 0, Bu = 0, Zeni = 5 }, Rarity = new GearRarity() { MinRarity = 1, MaxRarity = 1 } },
                new Gear() { Alias = "umbrella", Name = "Umbrella", Description = @"Normally made of wicker and silk, umbrellas keep both the sun and the rain off of those who carry them. Stories tell of umbrellas being used as improvised weapons by masters of the sword, though such fantastical feats would prove difficult for most to replicate.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 245 }, Cost = new MoneySum() { Koku = 0, Bu = 2, Zeni = 0 }, Rarity = new GearRarity() { MinRarity = 4, MaxRarity = 4 } },
                new Gear() { Alias = "whetstone", Name = "Whetstone", Description = @"Whetstones are portable grinding stones used to sharpen most bladed weapons, save for a samurai’s katana and wakizashi, which must be sharpened by an accomplished weaponsmith or professional polisher.", Server = dataServer, Source = new Source() { Book = "Core Rulebook", Page = 245 }, Cost = new MoneySum() { Koku = 0, Bu = 0, Zeni = 1 }, Rarity = new GearRarity() { MinRarity = 1, MaxRarity = 1 } }
            };
            ctx.Items.AddRange(gear);
            ctx.SaveChanges();
            await ReplyAsync("Done.");
        }
    }
}
