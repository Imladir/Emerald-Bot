using EmeraldBot.Model.Game;
using EmeraldBot.Model.Rolls;
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
    [Table("Characters")]
    public abstract class Character : NameAlias
    {
        [NotMapped]
        public static new readonly List<string> AcceptedFields = new List<string>() { "clan", "icon", "description", "any ring name"}.Concat(NameAlias.AcceptedFields).OrderBy(s => s).ToList();
        public string Icon { get; set; }
        public string Description { get; set; }
        public int Endurance { get; set; }
        public int Composure { get; set; }
        public int Focus { get; set; }
        public int Vigilance { get; set; }

        public virtual ICollection<Roll> Rolls { get; set; }
        public virtual ICollection<CharacterRing> Rings { get; set; }

        public Character()
        {
            Icon = "";
            Description = "";
        }

        public void InitRings(EmeraldBotContext ctx)
        {
            Rings = new List<CharacterRing>()
            {
                new CharacterRing() { Character = this, Ring = ctx.Rings.Single(x => x.Name.Equals("Air")), Value = 1 },
                new CharacterRing() { Character = this, Ring = ctx.Rings.Single(x => x.Name.Equals("Earth")), Value = 1 },
                new CharacterRing() { Character = this, Ring = ctx.Rings.Single(x => x.Name.Equals("Fire")), Value = 1 },
                new CharacterRing() { Character = this, Ring = ctx.Rings.Single(x => x.Name.Equals("Void")), Value = 1 },
                new CharacterRing() { Character = this, Ring = ctx.Rings.Single(x => x.Name.Equals("Water")), Value = 1 },
            };
        }

        public override void FullLoad(EmeraldBotContext ctx)
        {
            base.FullLoad(ctx);
            LoadRings(ctx);
        }

        public void LoadRings(EmeraldBotContext ctx)
        {
            ctx.Entry(this).Collection(x => x.Rings).Query()
                .Include(x => x.Ring).Load();
        }

        public virtual bool Add(NameAlias na)
        {
            return false;
        }

        public virtual bool Remove(NameAlias na)
        {
            return false;
        }

        public int Ring(string ring)
        {
            var cr = Rings.Single(x => x.Ring.Name == ring);
            return cr.Value;
        }

        public bool Ring(string ring, int value)
        {
            if (value < 1) throw new Exception("Ring values must be one or more.");
            if (value > 6) throw new Exception("Ring values can't be higher than 6");
            var cr = Rings.Single(x => x.Ring.Name.Equals(ring, StringComparison.OrdinalIgnoreCase));
            cr.Value = value;
            return true;
        }

        public void UpdateSecondaryStats()
        {
            // First, retrieve the ring values
            int air = Rings.Single(x => x.Character.Equals(this) && x.Ring.Name.Equals("Air")).Value;
            int earth = Rings.Single(x => x.Character.Equals(this) && x.Ring.Name.Equals("Earth")).Value;
            int fire = Rings.Single(x => x.Character.Equals(this) && x.Ring.Name.Equals("Fire")).Value;
            int water = Rings.Single(x => x.Character.Equals(this) && x.Ring.Name.Equals("Water")).Value;

            Focus = fire + air;
            Vigilance = (int)Math.Ceiling(((double)(air + water)) / 2);
            Endurance = 2 * (earth + fire);
            Composure = 2 * (earth + water);
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
                if (ctx.Rings.SingleOrDefault(x => x.Name.Equals(field)) != null) return Ring(field, int.Parse(value));
                else
                {
                    switch (field.ToLower())
                    {
                        case "description": Description = value; return true;
                        case "icon": Icon = value; return true;
                    }
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine($"Caught an exception on update for {field} with value {value}:\n{e.Message}\n{e.StackTrace}");
                throw new Exception($"Couldn't update {field} with value {value}: {e.Message}");
            }
            return false;
        }

        public static bool IsCharacter(EmeraldBotContext ctx, string aliasOrName)
        {
            return ctx.Characters.Count(x => x.Name.Equals(aliasOrName) || x.Alias.Equals(aliasOrName)) > 0;
        }
    }
}
