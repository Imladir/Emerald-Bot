using EmeraldBot.Model.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Characters
{
    [Table("NonPlayerCharacters")]
    public class NPC : Character
    {
        [NotMapped]
        public static new readonly List<string> AcceptedFields = new List<string>() { "composure", "endurance", "glory", "honour", "status", "demeanor", "type", "ability", "any skill group's name" }.Concat(Character.AcceptedFields).OrderBy(s => s).ToList();
        public virtual Demeanor Demeanor { get; set; }
        public virtual NPCType NPCType { get; set; }
        public int Honour { get; set; } = 0;
        public int Glory { get; set; } = 0;
        public int Status { get; set; } = 0;
        public int MartialRank { get; set; } = 1;
        public int SocialRank { get; set; } = 1;
        public string Ability { get; set; } = "";

        public virtual Source Source { get; set; }
        public virtual ICollection<NPCSkillGroup> SkillGroups { get; set; }

        public NPC() : base()
        {
        }

        public override void FullLoad(EmeraldBotContext ctx)
        {
            base.FullLoad(ctx);
            ctx.Entry(this).Reference(x => x.Demeanor).Load();
            ctx.Entry(this).Reference(x => x.NPCType).Load();
            ctx.Entry(this).Reference(x => x.Source).Load();
            ctx.Entry(this).Collection(x => x.SkillGroups).Load();
            foreach (var sg in SkillGroups) ctx.Entry(sg).Reference(x => x.SkillGroup).Load();
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
                if (base.UpdateField(ctx, field, value)) return true;
                else if (UpdateSkillGroup(ctx, field, int.Parse(value))) return true;
                else
                {
                    switch (field.ToLower())
                    {
                        case "composure": Composure = int.Parse(value); return true;
                        case "endurance": Endurance = int.Parse(value); return true;
                        case "honour": Honour = int.Parse(value); return true;
                        case "glory": Glory = int.Parse(value); return true;
                        case "status": Status = int.Parse(value); return true;
                        case "demeanor": Demeanor = ctx.Demeanors.Single(x => x.Name.Equals(value)); return true;
                        case "type": NPCType = ctx.NPCTypes.Single(x => x.Name.Equals(value)); return true;
                        case "ability": Ability = value; return true;
                        default: return false;
                    }
                }
            } catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateSkillGroup(EmeraldBotContext ctx, string name, int value)
        {
            ctx.Entry(this).Collection(x => x.SkillGroups).Load();
            var skillGroup = SkillGroups.SingleOrDefault(x => x.SkillGroupID == ctx.SkillGroups.Single(y => y.Name.Equals(name)).ID);
            if (skillGroup != null)
            {
                if (value > 0) skillGroup.Value = value;
                else SkillGroups.Remove(skillGroup);
                return true;
            }
            else
            {
                SkillGroups.Add(new NPCSkillGroup() { SkillGroup = ctx.SkillGroups.Single(x => x.Name.Equals(name)), Value = value });
                return true;
            }
        }

        public bool UpdateSkillGroup(SkillGroup sg, int value)
        {
            if (value < 0) throw new Exception("Skill group values can't be negative.");
            if (value > 6) throw new Exception("Skill group values can't be higher than 10");

            var skillGroup = SkillGroups.SingleOrDefault(x => x.SkillGroup == sg);
            if (skillGroup != null)
            {
                if (value > 0) skillGroup.Value = value;
                else SkillGroups.Remove(skillGroup);
            }
            else SkillGroups.Add(new NPCSkillGroup() { SkillGroup = sg, Value = value });
            return true;
        }
    }
}
