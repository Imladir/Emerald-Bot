using EmeraldBot.Model.Characters;
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
using System.Threading;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Rolls
{
    public class Roll
    {
        private static Random _rnd = new Random();
        private static DieFace RollingSkillDie;
        private static DieFace RollingRingDie;
        private EmeraldBotContext _ctx;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public virtual Server Server { get; set; }
        [Required]
        public virtual User Player { get; set; }
        [Required]
        public virtual Character Character { get; set; }

        [MaxLength(128, ErrorMessage = "Name is too long")]
        public string Name { get; set; } = "";
        public virtual Technique Technique { get; set; }
        public virtual Skill Skill { get; set; }
        public virtual Ring Ring { get; set; }
        public int TN { get; set; } = 0;
        [NotMapped]
        public bool Initial { get; set; }
        [Required]
        public bool Locked { get; set; }
        [Required]
        public long DiscordChannelID { get; set; } = 0;
        public long DiscordMessageID { get; set; } = 0;
        public string Log { get; set; } = "";
        [Required]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        [Required]
        public virtual ICollection<RollDie> Dice { get; set; }


        public Roll()
        {
            Locked = false;
            Dice = new List<RollDie>();
        }

        static Roll()
        {
            using var ctx = new EmeraldBotContext();
            RollingSkillDie = ctx.DieFaces.AsNoTracking().Single(x => x.DieType.Equals("Skill") && x.Value.Equals(""));
            RollingRingDie = ctx.DieFaces.AsNoTracking().Single(x => x.DieType.Equals("Ring") && x.Value.Equals(""));
        }

        private void SortDice()
        {
            Dice = Dice.OrderByDescending(x => x.Face.DieType).ToList();
        }

        public RollResult Score()
        {
            RollResult res = new RollResult();
            foreach (var d in Dice) res += d.Score();
            return res;
        }

        public RollPrintData EndRoll()
        {
            foreach (var d in Dice)
            {
                var type = d.Face.DieType;
                if (d.Face.Value.Equals(""))
                {
                    int nFaces = 6;
                    if (type.Equals("Skill")) nFaces = 12;
                    d.Face = _ctx.DieFaces.Where(x => x.DieType.Equals(type) && !x.Value.Equals("")).Skip(_rnd.Next(nFaces)).First();
                }
            }
            Initial = false;
            return PrintData();
        }

        public RollPrintData Reroll(EmeraldBotContext ctx, string reason = "")
        {
            return Reroll(ctx, Enumerable.Range(0, Dice.Count).ToList(), reason);
        }

        public RollPrintData Reroll(EmeraldBotContext ctx, List<int> ids, string reason = "")
        {
            _ctx = ctx;
            if (Locked) throw new Exception("Roll has been locked: you cannot modify it anymore.");
            reason = reason == "" ? "" : (": " + reason);
            _ctx.DieFaces.Attach(RollingRingDie);
            _ctx.DieFaces.Attach(RollingSkillDie);

            string rerolled = "";
            foreach (var i in ids)
            {
                if (Dice.ElementAt(i).Exploded == false)
                {
                    Emote em;
                    if (Dice.ElementAt(i).Face.Emote == null)
                    {
                        var query = from e in _ctx.Emotes
                                    join df in _ctx.DieFaces on e.ID equals df.Emote.ID
                                    where df.ID == Dice.ElementAt(i).Face.ID
                                    select e;
                        em = query.Single();
                    }
                    else
                        em = Dice.ElementAt(i).Face.Emote;

                    var type = Dice.ElementAt(i).Face.DieType;
                    Dice.ElementAt(i).Face = type.Equals("Skill") ? RollingSkillDie : RollingRingDie;
                    rerolled += "{" + em.Code + "}";
                }
            }

            if (!Initial) Log += $"Rerolled die {rerolled}{reason}\n";

            return PrintData();
        }

        public RollPrintData Replace(EmeraldBotContext ctx, List<int> indices, List<DieFace> dice, string reason = "")
        {
            _ctx = ctx;
            if (Locked) throw new Exception("Roll has been locked: you cannot modify it anymore.");
            reason = reason == "" ? "" : (": " + reason);

            string replaced = "";
            string with = "";

            foreach ((int i, DieFace face) in indices.Zip(dice))
            {
                replaced += "{" + Dice.ElementAt(i).Face.Emote.Code + "}";

                var query = from e in _ctx.Emotes
                            join df in _ctx.DieFaces on e.ID equals df.Emote.ID
                            where df.ID == face.ID
                            select e;
                Emote em = query.Single();

                with += "{" + em.Code + "}";
                Dice.ElementAt(i).Face = face;
            }

            Log += $"Replaced {replaced} with {with}{reason}\n";

            return PrintData();
        }

        public RollPrintData Add(EmeraldBotContext ctx, List<DieFace> dice, string reason = "")
        {
            _ctx = ctx;
            if (Locked) throw new Exception("Roll has been locked: you cannot modify it anymore.");
            reason = reason == "" ? "" : (": " + reason);

            Log += Initial ? "Created roll with " : "Added, then rolled ";

            foreach (var d in dice.OrderByDescending(x => x.DieType))
            {
                ctx.DieFaces.Attach(d);
                Log += "{" + d.DieType.ToLower()[0] + d.Value + "}";
                Dice.Add(new RollDie() { Face = d, Roll = this });
            }
            Log += reason + "\n";

            return PrintData();
        }

        public RollPrintData Explode(EmeraldBotContext ctx)
        {
            _ctx = ctx;
            if (Locked) throw new Exception("Roll has been locked: you cannot modify it anymore.");

            var added = new List<RollDie>();
            string explosions = "";
            _ctx.DieFaces.Attach(RollingRingDie);
            _ctx.DieFaces.Attach(RollingSkillDie);

            for (int i = 0; i < Dice.Count; i++)
            {
                if (Dice.ElementAt(i).Face.ExplosiveSuccess() && !Dice.ElementAt(i).Exploded)
                {
                    added.Add(new RollDie()
                    {
                        Face = Dice.ElementAt(i).Face.DieType.Equals("Skill") ? RollingSkillDie : RollingRingDie,
                        Roll = this,
                    });
                    explosions += "{" + Dice.ElementAt(i).Face.Emote.Code + "}";
                }
                Dice.ElementAt(i).Exploded = true;
            }
            if (added.Count > 0)
            {
                foreach (var d in added) Dice.Add(d);
                Log += $"Exploded {explosions}\n";
            }

            return PrintData();
        }

        public RollPrintData Keep(EmeraldBotContext ctx, List<int> kept, string reason = "")
        {
            _ctx = ctx;
            SortDice();
            if (Locked) throw new Exception("Roll has been locked: you cannot modify it anymore.");
            reason = reason == "" ? "" : (": " + reason);
            string keptStr = "";
            string droppedStr = "";

            for (int i = 0; i < Dice.Count; i++)
            {
                if (Dice.ElementAt(i).Exploded && !kept.Contains(i))
                    throw new Exception($"Cannot drop die {i+1}, it exploded already.");
                if (kept.Contains(i)) keptStr += "{" + Dice.ElementAt(i).Face.Emote.Code + "}";
                else droppedStr += "{" + Dice.ElementAt(i).Face.Emote.Code + "}";
            }

            Log += $"Kept {keptStr}";
            if (droppedStr != "") Log += $" (Dropped {droppedStr})";
            Log += reason + "\n";

            foreach (int i in Enumerable.Range(0, Dice.Count).Where(x => !kept.Contains(x)).Reverse()) Dice.Remove(Dice.ElementAt(i));

            return PrintData();
        }

        public RollPrintData Drop(EmeraldBotContext ctx, List<int> dropped, string reason = "")
        {
            return Keep(ctx, Enumerable.Range(0, Dice.Count).Where(x => !dropped.Contains(x)).ToList(), reason);
        }

        public string Lock(EmeraldBotContext ctx)
        {
            _ctx = ctx;
            if (Locked) throw new Exception("Roll is already locked");

            Locked = true;

            string diceStr = "";
            Dice.ToList().ForEach(x => diceStr += "{" + x.Face.Emote.Code + "}");

            var score = Score();
            string success = TN == 0 ? "obtained" : (score.CountSuccess() < TN ? "**failed** with" : "**succeeded** with");

            return $"{Character.Name} rolled {Name} with {(TN == 0 ? "Unknown TN" : $"TN{TN}")} and {success} {score.GetString(true)} ({diceStr})";
        }

        public RollPrintData SetTN(EmeraldBotContext ctx, int tn)
        {
            _ctx = ctx;
            if (Locked) throw new Exception("Roll has been locked: you cannot modify it anymore.");
            TN = tn;
            return PrintData();
        }

        public RollPrintData SetName(EmeraldBotContext ctx, string name)
        {
            _ctx = ctx;
            if (Locked) throw new Exception("Roll has been locked: you cannot modify it anymore.");
            Name = name;
            return PrintData();
        }

        public RollPrintData PrintData()
        {
            var score = Score();

            // The dice string
            string msg = "";
            SortDice();

            foreach (var d in Dice)
            {
                Console.WriteLine($"{_ctx}");
                var query = from e in _ctx.Emotes
                            join df in _ctx.DieFaces on e.ID equals df.Emote.ID
                            where df.ID == d.Face.ID
                            select e;
                Emote em = query.Single();

                msg += "{" + em.Code + "}";
            }

            RollPrintData res;

            res.Dice = msg;
            res.Title = $"{Character.Name}: {Name}\nTN{(TN != 0 ? $"{TN}" : " Unknown")}";
            res.Result = score.GetString();
            res.Log = Log;
            res.Icon = Character.Icon;
            res.State = TN == 0 ? 0 : (TN <= score.CountSuccess() ? 1 : -1);
            res.MessageID = DiscordMessageID;
            res.ChannelID = DiscordChannelID;
            res.IsRolling = Dice.Select(x => x.Face.Value).Any(x => x == "");

            return res;
        }
    }

    public struct RollPrintData
    {
        public string Dice;
        public string Title;
        public string Result;
        public string Log;
        public string Icon;
        public int State;
        public long MessageID;
        public long ChannelID;
        public bool IsRolling;
    }
}
