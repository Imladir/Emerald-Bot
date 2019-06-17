using EmeraldBot.Model.Game;
using EmeraldBot.Model.Identity;
using EmeraldBot.Model.Servers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Characters
{
    [Table("PlayerCharacters")]
    public class PC : Character
    {
        [NotMapped]
        public static new readonly List<string> AcceptedFields = new List<string>()
            { "age", "clan", "family", "school", "rank", "description", "ninjo", "giri", "add", "remove", "any skill name"}.Concat(Character.AcceptedFields).OrderBy(s => s).ToList();

        public virtual User Player { get; set; }
        public virtual Clan Clan { get; set; }
        public string Family { get; set; }
        public string School { get; set; }
        public int Rank { get; set; }
        public int Age { get; set; }

        [MaxLength(256, ErrorMessage = "Ninjo is too long")]
        public string Ninjo { get; set; }

        [MaxLength(256, ErrorMessage = "Giri is too long")]
        public string Giri { get; set; }

        [NotMapped]
        public int XP { get { return CurrentJournalValue("XP"); } }

        [NotMapped]
        public int Honour { get { return CurrentJournalValue("Honour"); } }

        [NotMapped]
        public int Glory { get { return CurrentJournalValue("Glory"); } }

        [NotMapped]
        public int Status { get { return CurrentJournalValue("Status"); } }

        public int CurrentVoid { get; set; }

        public virtual ICollection<PCAdvantage> Advantages { get; set; }
        public virtual ICollection<PCTechnique> Techniques { get; set; }
        public virtual ICollection<PCSkill> Skills { get; set; }
        public virtual ICollection<JournalEntry> JournalEntries { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

        public PC() : base()
        {
            Family = "";
            School = "";
            Rank = 1;
            Age = -1;
            Ninjo = "";
            Giri = "";
            Advantages = new List<PCAdvantage>();
            Techniques = new List<PCTechnique>();
            Skills = new List<PCSkill>();
            JournalEntries = new List<JournalEntry>();
        }

        public override void FullLoad(EmeraldBotContext ctx)
        {
            ctx.Entry(this).State = EntityState.Deleted;
            base.FullLoad(ctx);
            LoadClan(ctx);
            LoadPlayer(ctx);
            LoadAdvantages(ctx);
            LoadSkills(ctx);
            LoadTechniques(ctx);
            LoadJournals(ctx);
        }

        public void LoadClan(EmeraldBotContext ctx)
        {
            ctx.Entry(this).Reference(x => x.Clan).Load();
        }

        public void LoadPlayer(EmeraldBotContext ctx)
        {
            ctx.Entry(this).Reference(x => x.Player).Load();
        }

        public void LoadAdvantages(EmeraldBotContext ctx)
        {
            ctx.Entry(this).Collection(x => x.Advantages).Query()
                .Include(x => x.Advantage).ThenInclude(x => x.AdvantageClass)
                .Include(x => x.Advantage).ThenInclude(x => x.AdvantageTypes)
                .Load();

            foreach (var a in Advantages.Select(x => x.Advantage).ToList())
            {
                ctx.Entry(a).Collection(x => x.AdvantageTypes)
                    .Query().Include(x => x.AdvantageType).Load();
            }
        }
        public void LoadSkills(EmeraldBotContext ctx)
        {
            ctx.Entry(this).Collection(x => x.Skills).Load();
            foreach (var a in Skills) ctx.Entry(a).Reference(x => x.Skill).Load();
        }

        public void LoadTechniques(EmeraldBotContext ctx)
        {
            ctx.Entry(this).Collection(x => x.Techniques).Query()
                .Include(x => x.Technique).ThenInclude(x => x.Ring)
                .Include(x => x.Technique).ThenInclude(x => x.Skills)
                .Include(x => x.Technique).ThenInclude(x => x.Type)
                .Load();

            foreach (var t in Techniques.Select(x => x.Technique))
            {
                ctx.Entry(t).Collection(x => x.SkillGroups).Load();
                ctx.Entry(t).Collection(x => x.Skills).Query()
                    .Include(x => x.Skill).ThenInclude(x => x.Group)
                    .Load();
            }
        }
        public void LoadJournals(EmeraldBotContext ctx)
        {
            ctx.Entry(this).Collection(x => x.JournalEntries).Query()
                .Include(x => x.Journal)
                .Load();
        }

        public override bool Add(NameAlias na)
        {
            if (base.Add(na)) return true;
            else if (na is Technique) return Add(na as Technique);
            else if (na is Advantage) return Add(na as Advantage);
            else if (na is Skill) return Add(na as Skill);
            else return false;
        }

        public override bool Remove(NameAlias na)
        {
            if (base.Remove(na)) return true;
            else if (na is Technique) return Remove(na as Technique);
            else if (na is Advantage) return Remove(na as Advantage);
            else if (na is Skill) return Remove(na as Skill);
            else return false;
        }

        public void ResetVoid()
        {
            int voidRing = Rings.Single(x => x.Character.Equals(this) && x.Ring.Name.Equals("Void")).Value;
            CurrentVoid = (int)Math.Ceiling(((double)voidRing) / 2);
        }

        public override void Update(EmeraldBotContext ctx, Dictionary<string, string> args)
        {
            foreach (var kv in args)
                if (!base.UpdateField(ctx, kv.Key, kv.Value)) UpdateField(ctx, kv.Key, kv.Value);
        }

        public override bool UpdateField(EmeraldBotContext ctx, string field, string value)
        {
            try
            {
                var server = ctx.Servers.AsNoTracking().Single(x => x.NameAliases.Any(y => y.ID == ID));
                var na = ctx.GetNameAliasEntity((ulong)server.DiscordID, field);

                if (na != null)
                {
                    if (!int.TryParse(value, out int intValue)) throw new Exception($"Couldn't format {value} to integer.");

                    if (na is Skill) return Update(na as Skill, intValue);
                    throw new NotImplementedException("The developper forgot about something there...");
                }
                else if (base.UpdateField(ctx, field, value)) return true;
                else
                {
                    switch (field.ToLower())
                    {
                        case "clan": Clan = ctx.Clans.SingleOrDefault(x => x.Name.Equals(value)); return true;
                        case "family": Family = value; return true;
                        case "school": School = value; return true;
                        case "ninjo": Ninjo = value; return true;
                        case "giri": Giri = value; return true;
                        case "rank": Rank = int.Parse(value); return true;
                        case "age": Age = int.Parse(value); return true;
                        default: return false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Caught an exception on update for {field} with value {value}:\n{e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        public bool Update(Skill s, int value)
        {
            if (value < 0) throw new Exception("Skill values can't be negative.");
            if (value > 6) throw new Exception("Skill values can't be higher than 10");

            var skill = Skills.SingleOrDefault(x => x.Skill == s);
            if (skill != null)
            {
                if (value > 0) skill.Value = value;
                else Skills.Remove(skill);
            }
            else Skills.Add(new PCSkill() { Skill = s, Value = value });
            return true;
        }

        public bool UpdateSkill(EmeraldBotContext ctx, string aliasOrName, int value)
        {
            var skill = ctx.Skills.SingleOrDefault(x => (x.Name.Equals(aliasOrName) || x.Alias.Equals(aliasOrName)) 
                                                     && (x.Server.DiscordID == (long)Server.DiscordID || x.Server.DiscordID == 0));
            if (skill == null) return false;

            return Update(skill, value);
        }

        public bool Add(Advantage adv) {
            if (Advantages.Any(x => x.Advantage == adv)) return false;
            Advantages.Add(new PCAdvantage() { Advantage = adv, Character = this });
            return true;
        }
        public bool Add(Technique t)
        {
            if (Techniques.Any(x => x.Technique == t)) return false;
            Techniques.Add(new PCTechnique() { Technique = t, Character = this });
            return true;
        }
        public bool Add(Skill s)
        {
            if (Skills.Any(x => x.Skill == s)) return false;
            Skills.Add(new PCSkill() { Skill = s, PC = this, Value = 1 });
            return true;
        }
        public bool Remove(Advantage adv) {
            var res = Advantages.SingleOrDefault(x => x.AdvantageID == adv.ID);
            if (res == null) return false;

            return Advantages.Remove(res);
        }

        public bool Remove(Technique t)
        {
            var res = Techniques.SingleOrDefault(x => x.TechniqueID == t.ID);
            if (res == null) return false;

            return Techniques.Remove(res);
        }
        public bool Remove(Skill s) { return Skills.Remove(Skills.SingleOrDefault(x => x.Skill == s)); }

        public bool AddAdvantage(EmeraldBotContext ctx, string alias) { return Add(Advantage.Get(ctx, alias)); }
        public bool RemoveAdvantage(EmeraldBotContext ctx, string alias)
        {
            ctx.Entry(this).Collection(x => x.Advantages).Load();
            return Remove(Advantage.Get(ctx, alias));
        }
        public bool AddTechnique(EmeraldBotContext ctx, string alias) { return Add(Technique.Get(ctx, alias)); }
        public bool RemoveTechnique(EmeraldBotContext ctx, string alias)
        {
            ctx.Entry(this).Collection(x => x.Techniques).Load();
            return Remove(Technique.Get(ctx, alias));
        }

        public int CurrentJournalValue(string type)
        {
            using var ctx = new EmeraldBotContext();
            ctx.PCs.Attach(this);
            ctx.Entry(this).Collection(x => x.JournalEntries).Query()
                .Include(x => x.Journal)
                .Load();

            var journalType = ctx.JournalTypes.Single(x => x.Name == type);
            return CurrentJournalValue(journalType);
        }

        public int CurrentJournalValue(JournalType type)
        {
            var entries = JournalEntries.Where(x => x.Journal == type);
            if (entries.Count() == 0) return 0;
            else return entries.Sum(x => x.Amount);
        }
    }
}
