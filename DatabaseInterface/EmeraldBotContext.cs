using EmeraldBot.Model.Servers;
using EmeraldBot.Model.Game;
using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Rolls;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using GenericServices;
using System.Collections;

namespace EmeraldBot.Model
{
    public class EmeraldBotContext : DbContext
    {
        public static string ConnectionString;
        // Characters
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<CharacterCondition> CharacterConditions { get; set; }
        public DbSet<Demeanor> Demeanors { get; set; }
        public DbSet<JournalType> JournalTypes { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<PC> PCs { get; set; }
        public DbSet<NPC> NPCs { get; set; }
        public DbSet<NPCType> NPCTypes { get; set; }
        public DbSet<CharacterRing> CharacterRings { get; set; }
        public DbSet<CharacterSkill> CharacterSkills { get; set; }
        public DbSet<CharacterSkillGroup> CharacterSkillGroups { get; set; }

        // Game
        public DbSet<Armour> Armours { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<WeaponGrip> WeaponGrips { get; set; }
        public DbSet<WeaponType> WeaponTypes { get; set; }
        public DbSet<AdvantageClass> AdvantageClasses { get; set; }
        public DbSet<AdvantageType> AdvantageTypes { get; set; }
        public DbSet<Advantage> Advantages { get; set; }
        public DbSet<Game.Action> Actions { get; set; }
        public DbSet<ActionType> ActionTypes { get; set; }
        public DbSet<Clan> Clans { get; set; }
        public DbSet<Ring> Rings { get; set; }
        public DbSet<SkillGroup> SkillGroups { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Technique> Techniques { get; set; }
        public DbSet<TechniqueSkill> TechniquesSkills { get; set; }
        public DbSet<TechniqueSkillGroup> TechniqueSkillGroups { get; set; }
        public DbSet<TechniqueType> TechniqueTypes { get; set; }
        public DbSet<Opportunity> Opportunities { get; set; }
        public DbSet<OpportunityTrigger> OpportunityTriggers { get; set; }
        public DbSet<SceneType> SceneTypes { get; set; }
        public DbSet<ConflictType> ConflictTypes { get; set; }

        // Rolls
        public DbSet<DieFace> DieFaces { get; set; }
        public DbSet<Roll> Rolls { get; set; }

        //Servers
        public DbSet<NameAlias> NameAliases { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<Player> Players { get; set; }
        //public DbSet<DefaultCharacter> DefaultCharacters { get; set; }
        public DbSet<PrivateChannel> PrivateChannels { get; set; }
        public DbSet<Emote> Emotes { get; set; }
        public DbSet<Message> Messages { get; set; }

        public EmeraldBotContext(DbContextOptions<EmeraldBotContext> options)
          : base(options)
        { }

        public EmeraldBotContext() : base()
        {
            //Important performance code
            //Configuration.AutoDetectChangesEnabled = true;
            //Configuration.LazyLoadingEnabled = false;
            //Configuration.ProxyCreationEnabled = false;
        }

        static EmeraldBotContext()
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<EmeraldBotContext, EmeraldBotInitializer>());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString)
                .EnableDetailedErrors()
                .UseLazyLoadingProxies(true)
                .EnableSensitiveDataLogging()
                .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.QueryClientEvaluationWarning));
            base.OnConfiguring(optionsBuilder);
        }

        public static EmeraldBotContext Create()
        {
            return new EmeraldBotContext();
        }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            //mb.Entity<CharacterCondition>()
            //    .HasRequired(x => x.Character)
            //    .WithMany(x => x.Conditions)
            //    .HasForeignKey(x => x.CharacterID);

            //mb.Entity<CharacterCondition>()
            //    .HasRequired(x => x.Condition)
            //    .WithMany(x => x.Characters)
            //    .HasForeignKey(x => x.ConditionID);


            mb.Entity<CharacterRing>().HasKey(x => new { x.CharacterID, x.RingID });
            mb.Entity<CharacterSkill>().HasKey(x => new { x.PCID, x.SkillID });
            mb.Entity<CharacterSkillGroup>().HasKey(x => new { x.NPCID, x.SkillGroupID });
            mb.Entity<CharacterCondition>().HasKey(x => new { x.CharacterID, x.ConditionID });

            mb.Entity<PCAdvantage>().HasKey(x => new { x.CharacterID, x.AdvantageID });
            mb.Entity<PCTechnique>().HasKey(x => new { x.CharacterID, x.TechniqueID });
            mb.Entity<ActionScenes>().HasKey(x => new { x.ActionID, x.SceneTypeID });
            mb.Entity<ConflictAction>().HasKey(x => new { x.ConflictID, x.ActionID });
            mb.Entity<ActionTypeAction>().HasKey(x => new { x.ActionTypeID, x.ActionID });
            mb.Entity<AdvantageTypeAdvantages>().HasKey(x => new { x.AdvantageTypeID, x.AdvantageID });
            mb.Entity<GearQualitiesGear>().HasKey(x => new { x.GearID, x.GearQualityID });
            mb.Entity<OpportunityTriggerOpportunity>().HasKey(x => new { x.OpportunityID, x.OpportunityTriggerID });
            mb.Entity<TechniqueSkill>().HasKey(x => new { x.TechniqueID, x.SkillID });
            mb.Entity<TechniqueSkillGroup>().HasKey(x => new { x.TechniqueID, x.SkillGroupID });
            mb.Entity<WeaponGripsWeapon>().HasKey(x => new { x.WeaponID, x.WeaponGripID });
            mb.Entity<DefaultCharacter>().HasKey(x => new { x.ServerID, x.PlayerID });
            mb.Entity<GM>().HasKey(x => new { x.ServerID, x.PlayerID });
            mb.Entity<PrivateChannel>().HasKey(x => new { x.PlayerID, x.ServerID });
            //mb.Entity<DieType>().HasOne(x => x.Blank).WithMany(x => x.DieTypeBlanks);
            //mb.Entity<DieType>().HasOne(x => x.Gif).WithMany(x => x.DieTypeGifs);
            //mb.Entity<DieDefinition>().HasOne(x => x.Emote).WithMany(x => x.Dice);

            // mb.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            foreach (var relationship in mb.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(mb);
        }

        public bool CheckPrivateRights(ulong serverID, ulong playerID, int characterID)
        {
            var cmd = Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "usp_checkPrivateRights";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@serverDiscordID", SqlDbType.BigInt) { Value = serverID });
            cmd.Parameters.Add(new SqlParameter("@userDiscordID", SqlDbType.BigInt) { Value = playerID });
            cmd.Parameters.Add(new SqlParameter("@characterID", SqlDbType.Int) { Value = characterID });
            cmd.Parameters.Add(new SqlParameter("@hasRight", SqlDbType.Bit) { Direction = ParameterDirection.Output });

            if (cmd.Connection.State != ConnectionState.Open)
            {
                cmd.Connection.Open();
            }

            cmd.ExecuteNonQuery();

            return (bool)cmd.Parameters["@hasRight"].Value;
        }

        public bool CheckPrivateChannel(ulong serverID, ulong playerID, ulong chanID, ulong ownerID)
        {
            var cmd = Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "usp_checkPrivateChannel";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@serverID", SqlDbType.BigInt) { Value = serverID });
            cmd.Parameters.Add(new SqlParameter("@userID", SqlDbType.BigInt) { Value = playerID });
            cmd.Parameters.Add(new SqlParameter("@chanID", SqlDbType.BigInt) { Value = chanID });
            cmd.Parameters.Add(new SqlParameter("@ownerID", SqlDbType.BigInt) { Value = ownerID });
            cmd.Parameters.Add(new SqlParameter("@isPrivate", SqlDbType.Bit) { Direction = ParameterDirection.Output });

            if (cmd.Connection.State != ConnectionState.Open)
            {
                cmd.Connection.Open();
            }

            cmd.ExecuteNonQuery();

            return (bool)cmd.Parameters["@isPrivate"].Value;
        }

        public PC GetDefaultCharacter(ulong serverDiscordID, ulong playerDiscordID) { return GetPlayerCharacter(serverDiscordID, playerDiscordID, ""); }

        public PC GetPlayerCharacter(ulong serverID, ulong userID, string nameOrAlias)
        {
            var cmd = Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "usp_getUserCharacter";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@serverID", SqlDbType.BigInt) { Value = serverID });
            cmd.Parameters.Add(new SqlParameter("@userID", SqlDbType.BigInt) { Value = userID });
            cmd.Parameters.Add(new SqlParameter("@nameOrAlias", SqlDbType.NVarChar) { Value = nameOrAlias });
            cmd.Parameters.Add(new SqlParameter("@pcID", SqlDbType.Int) { Direction = ParameterDirection.Output });

            if (cmd.Connection.State != ConnectionState.Open)
            {
                cmd.Connection.Open();
            }

            cmd.ExecuteNonQuery();
            var id = (int)cmd.Parameters["@pcID"].Value;

            return PCs.Find(id);
        }

        public Character GetCommandTarget(ulong serverID, ulong userID, string aliasOrName = "")
        {
            Character target;
            if (Players.Single(x => x.DiscordID == (long)userID).IsGM(serverID) && aliasOrName != "")
                target = Characters.Where(x => (x.Name.Equals(aliasOrName) || x.Alias.Equals(aliasOrName))
                                             && (x.Server.DiscordID == (long)serverID || x.Server.DiscordID == 0)).Single();
            else
                target = GetPlayerCharacter(serverID, userID, aliasOrName);
            return target;
        }

        public NameAlias GetNameAliasEntity(ulong serverID, string nameOrAlias)
        {
            return NameAliases.AsNoTracking().FromSqlInterpolated($"usp_getNameAliasEntity {(long)serverID}, {nameOrAlias}").SingleOrDefault();
        }

        #region Save Changes
        public override int SaveChanges()
        {
            try
            {
                var aliasChangeSet = ChangeTracker.Entries<NameAlias>();
                if (aliasChangeSet != null)
                {
                    foreach (var entry in aliasChangeSet.Where(x => x.State != EntityState.Unchanged))
                    {
                        var server =
                            from s in Servers
                            join a in NameAliases on s.ID equals a.Server.ID
                            where a.ID == entry.Entity.ID
                            select s;

                        var na = GetNameAliasEntity((ulong)server.Single().DiscordID, entry.Entity.Alias);
                        if (na != null && na.ID != entry.Entity.ID)
                            throw new ValidationException($"there is already something registered with the alias '{na.Alias}'. Pick something else.");
                    }
                }

                // Updating PC secondary characteristics, or a NPC at creation
                var characterRingChangeSet = ChangeTracker.Entries<CharacterRing>().Where(x => x.State != EntityState.Unchanged).ToList();
                if (characterRingChangeSet != null)
                {
                    for (int i = 0; i < characterRingChangeSet.Count; i++)
                    {
                        Entry(characterRingChangeSet[i].Entity).Reference(x => x.Character).Load();
                        if (characterRingChangeSet[i].Entity.Character is PC || characterRingChangeSet[i].State == EntityState.Added)
                            characterRingChangeSet[i].Entity.Character.UpdateSecondaryStats();
                        if (characterRingChangeSet[i].Entity.Character is PC && characterRingChangeSet[i].State == EntityState.Added)
                            (characterRingChangeSet[i].Entity.Character as PC).ResetVoid();
                    }
                }

                var rollChanges = ChangeTracker.Entries<Roll>().Where(x => x.State != EntityState.Unchanged).ToList();
                if (rollChanges != null)
                {
                    for (int i = 0; i < rollChanges.Count; i++)
                    {
                        rollChanges[i].Entity.LastUpdated = DateTime.UtcNow;
                    }
                }

                var playerChanges = ChangeTracker.Entries<Player>().Where(x => x.State != EntityState.Unchanged).ToList();
                if (playerChanges != null)
                {
                    for (int i = 0; i < playerChanges.Count; i++)
                    {
                        playerChanges[i].Entity.LastUpdated = DateTime.UtcNow;
                    }
                }

                var res = base.SaveChanges();

                return res;
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine("That's a database exception alright");
                //This either returns a error string, or null if it can’t handle that error
                var error = SaveChangesExceptionHandler(e, this);
                if (error != null)
                {
                    Console.WriteLine(error); //return the error string
                }
                throw; //couldn’t handle that error, so rethrow
            }
            catch (Exception e)
            {
                if (e is ValidationException)
                    Console.WriteLine($"{(e as ValidationException).ValidationAttribute} with value {(e as ValidationException).Value} failed: {(e as ValidationException).ValidationResult}\n{e.Message}\n{e.StackTrace}");
                else
                    Console.WriteLine($"Exception in SaveChanges: \n{e.Message}\n{e.StackTrace}");
                var inner = e.InnerException;
                while (inner != null)
                {
                    if (e is ValidationException)
                        Console.WriteLine($"{(e as ValidationException).ValidationAttribute} with value {(e as ValidationException).Value} failed: {(e as ValidationException).ValidationResult}\n{e.Message}\n{e.StackTrace}");
                    else
                        Console.WriteLine($"Exception in SaveChanges: \n{e.Message}\n{e.StackTrace}");
                    inner = inner.InnerException;
                }
                throw;
            }
        }

        public const int SqlServerViolationOfUniqueIndex = 2601;
        public const int SqlServerViolationOfUniqueConstraint = 2627;

        public static IStatusGeneric SaveChangesExceptionHandler(Exception e, DbContext context)
        {
            var dbUpdateEx = e as DbUpdateException;
            if (dbUpdateEx?.InnerException is SqlException sqlEx)
            {
                Console.WriteLine($"SQL Exception #{sqlEx.Number}: {sqlEx.Message}");
                //This is a DbUpdateException on a SQL database

                //if (sqlEx.Number == SqlServerViolationOfUniqueIndex ||
                //    sqlEx.Number == SqlServerViolationOfUniqueConstraint)
                //{
                //We have an error we can process
                var valError = UniqueErrorFormatter(sqlEx, dbUpdateEx.Entries);
                if (valError != null)
                {
                    var status = new StatusGenericHandler();
                    status.AddValidationResult(valError);
                    return status;
                }
                //else check for other SQL errors
                //}
            }
            return null;
        }

        private static readonly Regex UniqueConstraintRegex = new Regex("'UniqueError_([a-zA-Z0-9]*)_([a-zA-Z0-9]*)'", RegexOptions.Compiled);

        public static ValidationResult UniqueErrorFormatter(SqlException ex, IReadOnlyList<EntityEntry> entitiesNotSaved)
        {
            var message = ex.Errors[0].Message;
            var matches = UniqueConstraintRegex.Matches(message);

            if (matches.Count == 0)
                return null;

            //currently the entitiesNotSaved is empty for unique constraints - see https://github.com/aspnet/EntityFrameworkCore/issues/7829
            var entityDisplayName = entitiesNotSaved.Count == 1
                ? entitiesNotSaved.Single().Entity.GetType().FullName
                : matches[0].Groups[1].Value;

            var returnError = "Cannot have a duplicate " +
                              matches[0].Groups[2].Value + " in " +
                              entityDisplayName + ".";

            var openingBadValue = message.IndexOf("(");
            if (openingBadValue > 0)
            {
                var dupPart = message.Substring(openingBadValue + 1,
                    message.Length - openingBadValue - 3);
                returnError += $" Duplicate value was '{dupPart}'.";
            }

            return new ValidationResult(returnError, new[] { matches[0].Groups[2].Value });
        }
        #endregion

        public void Seed()
        {
            using var dbTransaction = Database.BeginTransaction();
            try
            {
                Server server = new Server() { Prefix = "!", DiscordID = 0, DiceChannelID = 0, Name = "Game Data Server" };
                Servers.Add(server);

                IList<Condition> conditions = new List<Condition>
                {
                    new Condition()
                    {
                        Alias = "afflicted",
                        Name = "Afflicted",
                        Server = server,
                        Description = "The character is possessed or otherwise tormented by an onryō (vengeful ghost), kansen, or other malevolent spiritual entity.Given time, this evil force threatens to plant the seeds of the Shadowlands Taint."
                    },
                    new Condition()
                    {
                        Alias = "bleeding",
                        Name = "Bleeding",
                        Server = server,
                        Description = "The character is losing blood, internally or externally."
                    },
                    new Condition()
                    {
                        Alias = "burning",
                        Name = "Burning",
                        Server = server,
                        Description = "The character is ablaze."
                    },
                    new Condition()
                    {
                        Alias = "compromised",
                        Name = "Compromised",
                        Server = server,
                        Description = "The character is emotionally distraught and distracted."
                    },
                    new Condition()
                    {
                        Alias = "dazed",
                        Name = "Dazed",
                        Server = server,
                        Description = "The character’s vision is obscured or their focus is hampered, putting them on the defensive."
                    },
                    new Condition()
                    {
                        Alias = "disoriented",
                        Name = "Disoriented",
                        Server = server,
                        Description = "The character’s senses are scattered, cutting off their options to defend themself."
                    },
                    new Condition()
                    {
                        Alias = "dying",
                        Name = "Dying [X Rounds]",
                        Server = server,
                        Description = "The character is on the verge of death due to blood loss, organ failure, or another cause."
                    },
                    new Condition()
                    {
                        Alias = "enraged",
                        Name = "Enraged",
                        Server = server,
                        Description = "The character is consumed with rage and fights with a fury as devastating as it is terrifying."
                    },
                    new Condition()
                    {
                        Alias = "exhausted",
                        Name = "Exhausted",
                        Server = server,
                        Description = "The character is physically and mentally exhausted.Characters who go more than twenty-four hours without sleep(or less time under trying circumstances, at the GM’s discretion) suffer this condition"
                    },
                    new Condition()
                    {
                        Alias = "immobilized",
                        Name = "Immobilized",
                        Server = server,
                        Description = "The character is entangled, pinned down, or otherwise incapable of movement."
                    },
                    new Condition()
                    {
                        Alias = "incapacitated",
                        Name = "Incapacitated",
                        Server = server,
                        Description = "The character is largely incapable of action due to the scope of the harm they have suffered."
                    },
                    new Condition()
                    {
                        Alias = "intoxicated",
                        Name = "Intoxicated",
                        Server = server,
                        Description = "The character is drunk."
                    },
                    new Condition()
                    {
                        Alias = "prone",
                        Name = "Prone",
                        Server = server,
                        Description = "The character is flat on the ground. A character may voluntarily become Prone as a Movement action."
                    },
                    new Condition()
                    {
                        Alias = "silenced",
                        Name = "Silenced",
                        Server = server,
                        Description = "The character has been rendered temporarily speechless by shock or a swift blow."
                    },
                    new Condition()
                    {
                        Alias = "unconscious",
                        Name = "Unconscious",
                        Server = server,
                        Description = "The character has been knocked out, has fallen asleep, or has otherwise been rendered totally unaware of their surroundings."
                    },
                    new Condition()
                    {
                        Alias = "lightlyWounded",
                        Name = "Lightly Wounded",
                        Server = server,
                        Description = "A part of the character’s body is injured badly enough to hinder their use of it. This is reflected mechanically by one of their rings’ being negatively affected when the character uses it, which should be recorded along with the condition."
                    },
                    new Condition()
                    {
                        Alias = "severlyWounded",
                        Name = "Severly Wounded",
                        Server = server,
                        Description = "A part of the character’s body is injured badly enough to hinder their use of it. This is reflected mechanically by one of their rings’ being negatively affected when the character uses it, which should be recorded along with the condition."
                    }
                };
                Conditions.AddRange(conditions);
                SaveChanges();

                IList<Demeanor> demeanors = new List<Demeanor>
                    {
                        new Demeanor() { Name = "Ambitious", Effect = "Fire +2, Water –2", Unmasking = "Bend Principles" },
                        new Demeanor() { Name = "Assertive", Effect = "Earth +2, Air –2", Unmasking = "Rage" },
                        new Demeanor() { Name = "Detached", Effect = "Earth +1, Fire +1, Void –2", Unmasking = "Expose an Opening" },
                        new Demeanor() { Name = "Gruff", Effect = "Water +2, Earth –2", Unmasking = "Inappropriate Outburst" },
                        new Demeanor() { Name = "Shrewd", Effect = "Air +2, Fire –2", Unmasking = "Panicked Retreat" }
                    };
                Demeanors.AddRange(demeanors);

                IList<JournalType> journals = new List<JournalType>()
                    {
                        new JournalType() { Name = "XP" },
                        new JournalType() { Name = "Honour" },
                        new JournalType() { Name = "Glory" },
                        new JournalType() { Name = "Status" }
                    };
                JournalTypes.AddRange(journals);

                IList<NPCType> npcTypes = new List<NPCType>()
                    {
                        new NPCType() { Name = "Adversary" },
                        new NPCType() { Name = "Minion" }
                    };
                NPCTypes.AddRange(npcTypes);

                IList<ActionType> actionTypes = new List<ActionType>()
                    {
                        new ActionType() { Name = "Attack", Description = @"Attack actions are actions that a character can use to inflict harm on another character. An Attack action usually specifies at least one target, who suffers any relevant effects.
To affect another character with an Attack action, a character must generally have line of sight and an unobstructed path to that target." },
                        new ActionType() { Name = "Movement", Description = @"Movement actions are actions that allow a character to reposition in their environment.
To perform a Movement action, a character must usually have a path to the point they are trying to reach. Characters who can fly or scale sheer surfaces (via invocations or other supernatural abilities) are naturally more able to reach places earthbound characters cannot." },
                        new ActionType() { Name = "Scheme", Description = @"Scheme actions allow a character to attempt to influence targets with whom they can communicatemanipulating them, wearing them down, or bringing them around to a different point of view. A Scheme action usually specifies at least one target who suffers any relevant effects.
To affect another character with a Scheme action, a character must generally be able to communicate with that character, verbally or otherwise" },
                        new ActionType() { Name = "Support", Description = @"Support actions build up the character’s position or help others, protecting them or aiding them to set up for their own actions.
To affect another character with a Support action, a character must generally be able to communicate with that character, verbally or otherwise." }
                    };
                ActionTypes.AddRange(actionTypes);
                SaveChanges();

                IList<SceneType> scenes = new List<SceneType>()
                    {
                        new SceneType() { Name = "Narrative" },
                        new SceneType() { Name = "Downtime" },
                        new SceneType() { Name = "Conflict" }
                    };
                SceneTypes.AddRange(scenes);

                IList<ConflictType> conflictTypes = new List<ConflictType>()
                    {
                        new ConflictType() { Name = "Intrigue", Description = @"Intrigues are social engagements, chances to persuade the opposition to adopt one’s viewpoint, gather favor from superiors, sow the seeds of one’s plots, or subtly besmirch the reputation of hated rivals. Intrigues are how the fortunes of clans rise and fall. The power and influence of all ruling families rest in no small part on the skill of the courtiers who excel in this arena. 
Intrigues follow the general pattern described in Conflict Scenes on page 249, with the following minor additions and alterations:" },
                        new ConflictType() { Name = "Duel", Description = @"Duels are stylized engagements, usually fought to settle disputes that cannot be put to rest with words alone. A duel is fought by individuals, but the fate of families, clans, or the entire realm might ride on a duel." },
                        new ConflictType() { Name = "Skirmish", Description = @"Skirmishes are pitched battles between small groups of combatants—a few individuals, or small squads at the largest. This type of conflict can represent anything from a clash of scouting forces, to a teahouse raid with a few dozen combatants, to a pitched battle between several individuals in an alleyway. In a skirmish, the winner is the one who survives, whatever that might require." }
                    };
                SaveChanges();

                IList<Game.Action> actions = new List<Game.Action>()
                    {
                        new Game.Action() { Name = "Persuade", Description = @"You attempt to foster or quell an idea, emotion, or desire in a person (based on the approach you are using, as described in Social Skills, on page 151).", Activation = @"$As a Scheme action, you make a Social skill check targeting one or more characters who can hear you. The TN of the check is equal to the highest vigilance among your targets. Additionally, if applicable, apply one of the following modifications to the TN based on the skill you use on the check:
- Command: You present a plan with authority, sweeping your targets along with all the confidence a lord should have among their vassals. If each target’s status rank is lower than yours, decrease the TN of this check by 1.
- Courtesy: You present a proposal backed by honeyed words and clever implications—perfect for dealing with one’s superiors, but perhaps overly deferential when addressing equals or subordinates. If each target’s status rank is higher than yours, decrease the TN of this check by 1.
- Games/Performance/Other Skills: You offer a diversion from the affairs at hand, turning the conversation to a less pressing topic. If each target’s status rank is equal to yours, decrease the TN of this check by 1.", Effect = @"If you succeed, you add one momentum point toward an appropriate social objective, plus one additional momentum point for every two bonus successes. Further, any narrative ramifications of your check resolve."},
                        new Game.Action() { Name = "Center", Description = @"You let the world around you slow as the Void overtakes your senses. Everything but the moment fades as you instinctively seek victory. Your mind weaves through infinite treacherous possibilities and the razor steel of your foe to the outcome you desire.", Activation = @"As a Support action in Void stance, you may focus your energy inward, envisioning your action in your mind and seeking the perfect moment to take it. You must name a skill when you use Center.", Effect = @"Roll a number of Skill dice up to your ranks in the skill you chose and reserve any number of those dice. If you do, the next time you make a check using the chosen skill (or use the Center action) this scene, after rolling dice, you may replace any number of rolled dice with the reserved dice (set to the results they were showing when reserved). You cannot reserve a number of dice greater than your ranks in the skill this way."},
                        new Game.Action() { Name = "Predict", Description = @"You shift subtly to draw a reaction out of your foe by repositioning or signaling a strike you never intend to throw. By predicting your foe’s reaction, you aim to win the battle in the mind, leaving the foe open to a decisive strike or forcing them to attack early.", Activation = @"As an Attack and Scheme action, you may secretly select Air, Earth, Fire, or Water and record it.", Effect = @"The next time your opponent chooses their stance, you may reveal your selection; if it matches the stance they chose, your opponent receives 4 strife and must choose a different stance. This effect persists until the end of your next turn."},
                        new Game.Action() { Name = "Assist", Description = @"You offer an ally a plan of attack to use, an insight about the foe, or an encouraging word.", Activation = @"As an Attack, Scheme, and Support action, describe how you are helping one other character at range 0–2 with their next action.", Effect = @"If the GM accepts your suggestion, you provide assistance (see page 26) on the chosen character’s next action check."},
                        new Game.Action() { Name = "Calming Breath", Description = @"During a conflict, you may inhale deeply before exhaling, drawing upon your inner strength.", Activation = @"As a Support action, you may take a deep breath to calm yourself and recover stamina.", Effect = @"If your strife is greater than half your composure, you remove 1 strife. If your fatigue is greater than half your endurance, you remove 1 fatigue."},
                        new Game.Action() { Name = "Challenge", Description = @" You issue a challenge to a foe, calling for them to face you in single combat.", Activation = @"As a Scheme action, you may make a **TN 1 Command** check to issue a formal combat challenge targeting one character at range 0–5. You must stake 10 honor and 5 glory upon the challenge, which you forfeit if you sabotage the clash.", Effect = @"If you succeed, the target must choose whether to **accept** or **decline**; resolve one of the following:
- If the target **accepts**, they stake 10 honor and 5 glory, which they forfeit if they take any Attack or Scheme action before the clash. At the end of the round, the clash begins.
- To **decline**, the target must forfeit glory equal to your ranks in Command plus your bonus successes. Each of their allies with lower glory than you suffers 2 strife. Then, you gain 1 Void point. If you win the clash, each of your foe’s allies in the skirmish suffers 3 strife. If you lose the clash, each of your allies suffers 3 strife."},
                        new Game.Action() { Name = "Guard", Description = @"You focus on warding off foes from yourself or an ally by positioning yourself defensively, taking cover, throwing strategically placed strikes, or even firing shots menacingly close to the enemy.", Activation = @" As a Support action using a readied weapon, you may make a **TN 1 Tactics** check targeting yourself or one other character within the weapon’s range.", Effect = @" If you succeed, you **guard** the target until the beginning of your next turn. Increase the TN of Attack checks against the guarded target by one, plus an additional one per two bonus successes."},
                        new Game.Action() { Name = "Maneuver", Description = @"You shift on the battlefield, moving to a more advantageous position.", Activation = @" As a Movement action, you may reposition for more distance. Optionally, you may make a **TN 2 Fitness** check as part of this action.", Effect = @"Move one range band. If you choose to make the Fitness check and you succeed, you may instead move two range bands, plus one additional range band per two bonus successes."},
                        new Game.Action() { Name = "Prepare Item", Description = @"You prepare, ready, or stow one weapon or other item.", Activation = @"As a Support action, you may interact with one item.", Effect = @"Prepare one item for use, ready a weapon in a grip of your choice, or stow an item."},
                        new Game.Action() { Name = "Strike", Description = @"You make an attack against a single foe.", Activation = @"As an Attack action using one readied weapon, you may make a **TN 2 Martial Arts** check using the appropriate skill for the weapon, targeting one character within the weapon’s range.", Effect = @"If you succeed, you deal physical damage to the target equal to the weapon’s base damage plus your bonus successes."},
                        new Game.Action() { Name = "Unique Action", Description = @"You make a check using a skill for a mechanical or narrative effect, as described in **Chapter 3: Skills** (see page 140).", Activation = @"As an action, you make a skill check to attempt a task you have described to the GM.", Effect = @"If you succeed, you may use the skill for its narrative effects, for implementing any sample use that can be completed in a single action, or for pursuing another task that the GM deems appropriate."},
                        new Game.Action() { Name = "Wait", Description = @"You bide your time, waiting to spring into action.", Activation = @" As an Attack, Scheme, and Support action, you may declare a non-Movement action you will perform after the occurrence of a specified event before the end of the round.", Effect = @"After the specified event occurs before the end of the round, you may perform the action. You must still use the ring matching your stance for this action.
If the specified event does not occur this round, you may perform one action of your choice (other than Wait) at the end of the round."}
                    };
                Actions.AddRange(actions);
                SaveChanges();

                // Set conflict types
                actions[0].Conflicts.Add(new ConflictAction() { Action = actions[0], Conflict = conflictTypes[0] });
                actions[1].Conflicts.Add(new ConflictAction() { Action = actions[1], Conflict = conflictTypes[1] });
                actions[2].Conflicts.Add(new ConflictAction() { Action = actions[2], Conflict = conflictTypes[1] });
                actions[3].Conflicts.Add(new ConflictAction() { Action = actions[3], Conflict = conflictTypes[0] });
                actions[3].Conflicts.Add(new ConflictAction() { Action = actions[3], Conflict = conflictTypes[1] });
                actions[4].Conflicts.Add(new ConflictAction() { Action = actions[4], Conflict = conflictTypes[0] });
                actions[4].Conflicts.Add(new ConflictAction() { Action = actions[4], Conflict = conflictTypes[1] });
                actions[4].Conflicts.Add(new ConflictAction() { Action = actions[4], Conflict = conflictTypes[2] });
                actions[5].Conflicts.Add(new ConflictAction() { Action = actions[5], Conflict = conflictTypes[2] });
                actions[6].Conflicts.Add(new ConflictAction() { Action = actions[6], Conflict = conflictTypes[2] });
                actions[7].Conflicts.Add(new ConflictAction() { Action = actions[7], Conflict = conflictTypes[2] });
                actions[8].Conflicts.Add(new ConflictAction() { Action = actions[8], Conflict = conflictTypes[1] });
                actions[8].Conflicts.Add(new ConflictAction() { Action = actions[8], Conflict = conflictTypes[2] });
                actions[9].Conflicts.Add(new ConflictAction() { Action = actions[9], Conflict = conflictTypes[1] });
                actions[9].Conflicts.Add(new ConflictAction() { Action = actions[9], Conflict = conflictTypes[2] });
                actions[10].Conflicts.Add(new ConflictAction() { Action = actions[10], Conflict = conflictTypes[0] });
                actions[10].Conflicts.Add(new ConflictAction() { Action = actions[10], Conflict = conflictTypes[1] });
                actions[10].Conflicts.Add(new ConflictAction() { Action = actions[10], Conflict = conflictTypes[2] });
                actions[11].Conflicts.Add(new ConflictAction() { Action = actions[11], Conflict = conflictTypes[2] });

                // Set action types
                actions[0].Types.Add(new ActionTypeAction() { Action = actions[0], ActionType = actionTypes[2] });
                actions[1].Types.Add(new ActionTypeAction() { Action = actions[1], ActionType = actionTypes[3] });
                actions[2].Types.Add(new ActionTypeAction() { Action = actions[2], ActionType = actionTypes[0] });
                actions[2].Types.Add(new ActionTypeAction() { Action = actions[2], ActionType = actionTypes[2] });
                actions[3].Types.Add(new ActionTypeAction() { Action = actions[3], ActionType = actionTypes[0] });
                actions[3].Types.Add(new ActionTypeAction() { Action = actions[3], ActionType = actionTypes[2] });
                actions[3].Types.Add(new ActionTypeAction() { Action = actions[3], ActionType = actionTypes[3] });
                actions[4].Types.Add(new ActionTypeAction() { Action = actions[4], ActionType = actionTypes[3] });
                actions[5].Types.Add(new ActionTypeAction() { Action = actions[5], ActionType = actionTypes[2] });
                actions[6].Types.Add(new ActionTypeAction() { Action = actions[6], ActionType = actionTypes[3] });
                actions[7].Types.Add(new ActionTypeAction() { Action = actions[7], ActionType = actionTypes[1] });
                actions[8].Types.Add(new ActionTypeAction() { Action = actions[8], ActionType = actionTypes[3] });
                actions[9].Types.Add(new ActionTypeAction() { Action = actions[9], ActionType = actionTypes[0] });
                actions[11].Types.Add(new ActionTypeAction() { Action = actions[11], ActionType = actionTypes[0] });
                actions[11].Types.Add(new ActionTypeAction() { Action = actions[11], ActionType = actionTypes[2] });
                actions[11].Types.Add(new ActionTypeAction() { Action = actions[11], ActionType = actionTypes[3] });
                SaveChanges();

                IList<AdvantageClass> advantageClasses = new List<AdvantageClass>()
                    {
                        new AdvantageClass() { Name = "Distinction" },
                        new AdvantageClass() { Name = "Passion" },
                        new AdvantageClass() { Name = "Adversity" },
                        new AdvantageClass() { Name = "Anxiety" }
                    };
                AdvantageClasses.AddRange(advantageClasses);

                IList<AdvantageType> advantageTypes = new List<AdvantageType>()
                    {
                        new AdvantageType() { Name = "Curse" },
                        new AdvantageType() { Name = "Fame" },
                        new AdvantageType() { Name = "Flaw" },
                        new AdvantageType() { Name = "Infamy" },
                        new AdvantageType() { Name = "Interpersonal" },
                        new AdvantageType() { Name = "Mental" },
                        new AdvantageType() { Name = "Physical" },
                        new AdvantageType() { Name = "Scar" },
                        new AdvantageType() { Name = "Spiritual" },
                        new AdvantageType() { Name = "Virtue" }
                    };
                AdvantageTypes.AddRange(advantageTypes);
                SaveChanges();

                IList<Clan> clans = new List<Clan>()
                    {
                        new Clan() { Name = "Imperial", Colour = 3066993, Icon = "https://vignette.wikia.nocookie.net/l5r/images/5/5f/Imperial.png" },
                        new Clan() { Name = "Crab", Colour = 9127187, Icon = "https://vignette.wikia.nocookie.net/l5r/images/6/63/Crab.png" },
                        new Clan() { Name = "Crane", Colour = 3447003, Icon = "https://vignette.wikia.nocookie.net/l5r/images/2/20/Crane.png" },
                        new Clan() { Name = "Dragon", Colour = 2067276, Icon = "https://vignette.wikia.nocookie.net/l5r/images/c/c7/Dragon.png" },
                        new Clan() { Name = "Fox", Colour = 15105570, Icon = "https://vignette.wikia.nocookie.net/l5r/images/7/73/Fox.png" },
                        new Clan() { Name = "Lion", Colour = 15105570, Icon = "https://vignette.wikia.nocookie.net/l5r/images/4/47/Lion.png" },
                        new Clan() { Name = "Mantis", Colour = 32896, Icon = "https://vignette.wikia.nocookie.net/l5r/images/4/48/Mantis.png" },
                        new Clan() { Name = "Phoenix", Colour = 15158332, Icon = "https://vignette.wikia.nocookie.net/l5r/images/c/c1/Phoenix.png" },
                        new Clan() { Name = "Scorpion", Colour = 10038562, Icon = "https://vignette.wikia.nocookie.net/l5r/images/a/ab/Scorpion.png" },
                        new Clan() { Name = "Unicorn", Colour = 10181046, Icon = "https://vignette.wikia.nocookie.net/l5r/images/7/7e/Unicorn.png" },
                        new Clan() { Name = "", Colour = 12354406, Icon = "https://gamepedia.cursecdn.com/l5r_gamepedia_en/thumb/1/1f/Rings.png/300px-Rings.png" },
                        new Clan() { Name = "GM", Colour = 16777215, Icon = "https://gamepedia.cursecdn.com/l5r_gamepedia_en/thumb/1/1f/Rings.png/300px-Rings.png" }
                    };
                Clans.AddRange(clans);

                IList<Ring> rings = new List<Ring>()
                    {
                        new Ring() { Name = "Any" },
                        new Ring() { Name = "Air" },
                        new Ring() { Name = "Earth" },
                        new Ring() { Name = "Fire" },
                        new Ring() { Name = "Void" },
                        new Ring() { Name = "Water" },
                    };
                Rings.AddRange(rings);
                SaveChanges();

                IList<SkillGroup> skillGroups = new List<SkillGroup>()
                    {
                        new SkillGroup() { Name = "Artisan" },
                        new SkillGroup() { Name = "Martial" },
                        new SkillGroup() { Name = "Scholar" },
                        new SkillGroup() { Name = "Social" },
                        new SkillGroup() { Name = "Trade" }
                    };
                SkillGroups.AddRange(skillGroups);

                IList<TechniqueType> techniqueTypes = new List<TechniqueType>()
                    {
                        new TechniqueType() { Name = "Kata" },
                        new TechniqueType() { Name = "Kiho" },
                        new TechniqueType() { Name = "Invocation" },
                        new TechniqueType() { Name = "Ritual" },
                        new TechniqueType() { Name = "Shuji" },
                        new TechniqueType() { Name = "Maho" },
                        new TechniqueType() { Name = "Ninjutsu" }
                    };
                TechniqueTypes.AddRange(techniqueTypes);
                SaveChanges();

                IList<WeaponGrip> weaponGrips = new List<WeaponGrip>()
                    {
                        new WeaponGrip(1) {NewRangeMin = -1, NewRangeMax = -1, DamageModificator = 0, DeadlinessModificator = 0},
                        new WeaponGrip(1) {NewRangeMin = 1, NewRangeMax = 1, DamageModificator = 0, DeadlinessModificator = 0},
                        new WeaponGrip(2) {NewRangeMin = -1, NewRangeMax = -1, DamageModificator = 0, DeadlinessModificator = 0},
                        new WeaponGrip(2) {NewRangeMin = -1, NewRangeMax = -1, DamageModificator = 1, DeadlinessModificator = 0},
                        new WeaponGrip(2) {NewRangeMin = -1, NewRangeMax = -1, DamageModificator = 2, DeadlinessModificator = 0},
                        new WeaponGrip(2) {NewRangeMin = -1, NewRangeMax = -1, DamageModificator = 0, DeadlinessModificator = 1},
                        new WeaponGrip(2) {NewRangeMin = -1, NewRangeMax = -1, DamageModificator = 0, DeadlinessModificator = 2},
                        new WeaponGrip(2) {NewRangeMin = 2, NewRangeMax = 3, DamageModificator = 0, DeadlinessModificator = 0}
                    };
                WeaponGrips.AddRange(weaponGrips);

                IList<WeaponType> weaponTypes = new List<WeaponType>()
                    {
                        new WeaponType() { Name = "Swords" },
                        new WeaponType() { Name = "Axes" },
                        new WeaponType() { Name = "Blunt Weapons" },
                        new WeaponType() { Name = "Hand Weapons" },
                        new WeaponType() { Name = "Polearms" },
                        new WeaponType() { Name = "Bows" },
                        new WeaponType() { Name = "Crossbows" },
                        new WeaponType() { Name = "Specialist Weapons" },
                        new WeaponType() { Name = "Unarmed" }
                    };
                WeaponTypes.AddRange(weaponTypes);
                SaveChanges();


                IList<Emote> emotes = new List<Emote>()
                    {
                        new Emote() { Code = "success", Server = server, Text = "<:success:579762763124047884>" },
                        new Emote() { Code = "explosive", Server = server, Text = "<:explosiveSuccess:579762763191156737>" },
                        new Emote() { Code = "opportunity", Server = server, Text = "<:opportunity:579762763124047894>" },
                        new Emote() { Code = "strife", Server = server, Text = "<:strife:579762763090493450>" },
                        new Emote() { Code = "s*", Server = server, Text = "<a:whitegif:579762765166542884>" },
                        new Emote() { Code = "r*", Server = server, Text = "<a:blackgif:579762762977247264>" },
                        new Emote() { Code = "s", Server = server, Text = "<:white:579762763400871937>" },
                        new Emote() { Code = "r", Server = server, Text = "<:black:579762763199414281>" },
                        new Emote() { Code = "rot", Server = server, Text = "<:blackot:579762763488952330>" },
                        new Emote() { Code = "ro", Server = server, Text = "<:blacko:579762763719770172>" },
                        new Emote() { Code = "rst", Server = server, Text = "<:blackst:579762763685953566>" },
                        new Emote() { Code = "rs", Server = server, Text = "<:blacks:579762765179256842>" },
                        new Emote() { Code = "ret", Server = server, Text = "<:blacket:579762763233099776>" },
                        new Emote() { Code = "so", Server = server, Text = "<:whiteo:579762764952633354>" },
                        new Emote() { Code = "sst", Server = server, Text = "<:whitest:579762764898369557>" },
                        new Emote() { Code = "ss", Server = server, Text = "<:whites:579762764189270017>" },
                        new Emote() { Code = "sso", Server = server, Text = "<:whiteso:579762764776603665>" },
                        new Emote() { Code = "set", Server = server, Text = "<:whiteet:579762764722077706>" },
                        new Emote() { Code = "se", Server = server, Text = "<:whitee:579762764306972683>" }
                    };
                Emotes.AddRange(emotes);
                SaveChanges();

                IList<DieFace> dice = new List<DieFace>()
                    {
                        new DieFace() { DieType = "Ring", Value = "", Emote = emotes[5] },
                        new DieFace() { DieType = "Ring", Value = "-", Emote = emotes[7] },
                        new DieFace() { DieType = "Ring", Value = "ot", Emote = emotes[8] },
                        new DieFace() { DieType = "Ring", Value = "o", Emote = emotes[9] },
                        new DieFace() { DieType = "Ring", Value = "st", Emote = emotes[10] },
                        new DieFace() { DieType = "Ring", Value = "s", Emote = emotes[11] },
                        new DieFace() { DieType = "Ring", Value = "et", Emote = emotes[12] },
                        new DieFace() { DieType = "Skill", Value = "", Emote = emotes[4] },
                        new DieFace() { DieType = "Skill", Value = "-", Emote = emotes[6] },
                        new DieFace() { DieType = "Skill", Value = "-", Emote = emotes[6] },
                        new DieFace() { DieType = "Skill", Value = "o", Emote = emotes[13] },
                        new DieFace() { DieType = "Skill", Value = "o", Emote = emotes[13] },
                        new DieFace() { DieType = "Skill", Value = "o", Emote = emotes[13] },
                        new DieFace() { DieType = "Skill", Value = "st", Emote = emotes[14] },
                        new DieFace() { DieType = "Skill", Value = "st", Emote = emotes[14] },
                        new DieFace() { DieType = "Skill", Value = "s", Emote = emotes[15] },
                        new DieFace() { DieType = "Skill", Value = "s", Emote = emotes[15] },
                        new DieFace() { DieType = "Skill", Value = "so", Emote = emotes[16] },
                        new DieFace() { DieType = "Skill", Value = "et", Emote = emotes[17] },
                        new DieFace() { DieType = "Skill", Value = "e", Emote = emotes[18] }
                    };
                DieFaces.AddRange(dice);
                SaveChanges();

                // Everything went fine so commit
                dbTransaction.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Seeding failed: {e.Message}\n{e.StackTrace}");
                dbTransaction.Rollback();
            }
        }
    }
}
