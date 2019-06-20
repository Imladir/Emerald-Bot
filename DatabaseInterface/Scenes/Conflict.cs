using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Servers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Scenes
{
    public class Conflict
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public virtual Server Server { get; set; }
        public virtual ConflictType ConflictType { get; set; }
        public int Round { get; set; } = 0;
        public virtual ICollection<ConflictParticipant> Participants { get; set; } = new List<ConflictParticipant>();
        public int CurrentParticipant { get; set; } = -1;

        public ConflictParticipant GetCurrent()
        {
            if (Participants.Count > 0 && CurrentParticipant >= 0)
                return Participants.ElementAt(CurrentParticipant);
            else
                return null;
        }

        public string GetCurrentState(int i = -1)
        {
            OrderByInit();
            string res = "";
            if (Participants.Count == 0) throw new Exception("Conflict has no participants yet");
            else if (CurrentParticipant == -1 && i == -1) throw new Exception("Conflict hasn't startedd yet.");

            if (i == -1) i = CurrentParticipant;

            var current = Participants.ElementAt(i);

            if (current.Character != null && current.Character is PC pc)
            {
                if (pc.HasCondition("Compromised") && pc.HasCondition("Incapacitated"))
                    res = $"*Incapacitated* and *Compromised*.";
                else if (pc.HasCondition("Compromised"))
                    res = $"*Compromised*.";
                else if (pc.HasCondition("Incapacitated"))
                    res = $"*Incapacitated*.";
            }
            else if (current.Character != null && current.Character is NPC npc) // it's a known NPC profile
            {
                if (current.Fatigue > npc.Endurance && current.Strife > npc.Composure)
                    res = $"*Incapacitated* and *Compromised*.";
                else if (current.Strife > npc.Composure)
                    res = $"*Compromised*.";
                else if (current.Fatigue > npc.Endurance)
                    res = $"*Incapacitated*.";
            }

            return res;
        }

        public void AddParticipant(Character c, int init)
        {
            if (c == null) throw new Exception("You must use *!conflict add* to add random no name particpant.");
            var participant = new ConflictParticipant()
            {
                Character = c,
                Conflict = this,
                Init = init,
                Strife = 0,
                Fatigue = 0
            };

            if (c is PC pc) participant.Name = pc.Name;
            else if (c is NPC npc) participant.Name = $"{Participants.Count} - {npc.Name}";

            if (Round != 0 || CurrentParticipant != -1)
                participant.IsLate = true;

            Participants.Add(participant);
            OrderByInit();
        }

        public string AddParticipant(string name, int init)
        {
            var participant = new ConflictParticipant()
            {
                Character = null,
                Conflict = this,
                Init = init,
                Strife = 0,
                Fatigue = 0
            };

            if (name != "") participant.Name = name;
            else participant.Name = $"Bad guy #{Participants.Count}";

            if (Round != 0 || CurrentParticipant != -1)
                participant.IsLate = true;

            Participants.Add(participant);
            OrderByInit();

            return participant.Name;
        }

        public string RemoveParticipant(int i)
        {
            if (i < 0 || i >= Participants.Count) throw new Exception($"Cannot remove participant {i}. It must be between 1 and {Participants.Count}");

            OrderByInit();
            var c = Participants.ElementAt(i);
            Participants.Remove(c);
            return $"{c.Name} has been removed from the conflict!";
        }

        public bool Next()
        {
            OrderByInit();
            bool res = false;
            do
            {
                CurrentParticipant++;
            } while (CurrentParticipant < Participants.Count && Participants.ElementAt(CurrentParticipant).IsLate);

            if (CurrentParticipant == Participants.Count)
            {
                NextRound();
                res = true;
            }
            return res;
        }

        public void NextRound()
        {
            Round++;
            CurrentParticipant = 0;
            foreach (var p in Participants) p.IsLate = false;
        }

        public void OrderByInit()
        {
            Participants = Participants.OrderByDescending(x => x.Init).ThenBy(x => x.Character != null ? (x.Character is PC ? (x.Character as PC).Honour : (x.Character as NPC).Honour) : 0).ToList();
        }

        public string ModifyStrife(int userID, int target, int strife)
        {
            OrderByInit();
            using var ctx = new EmeraldBotContext();
            string res = "Strife has been applied.";
            if (target < 0 || target > Participants.Count - 1)
                throw new Exception($"The target number must be between 1 and {Participants.Count}.");

            Character c = Participants.ElementAt(target).Character;
            if (c is PC pc)
            {
                if (!ctx.CheckPrivateRights(Server.ID, userID, pc.ID)) throw new Exception("Only GMs or the character owner can modify its strife.");
                res += pc.ModifyStrife(ctx, strife);
            }
            else if (c is NPC npc)
            {
                if (!ctx.CheckPrivateRights(Server.ID, userID, npc.ID)) throw new Exception("Only GMs can modify an NPC's strife.");

                int threshold = npc.Composure;
                bool changed = false;
                if (strife > 0 && Participants.ElementAt(target).Strife <= threshold && Participants.ElementAt(target).Strife + strife > threshold) changed = true;
                else if (strife < 0 && Participants.ElementAt(target).Strife > threshold && Participants.ElementAt(target).Strife + strife <= threshold) changed = true;
                Participants.ElementAt(target).Strife += strife;
                if (changed && strife > 0) res += $" {Participants.ElementAt(target).Name} has become **Compromised**.";
                else if (changed && strife < 0) res += $" {Participants.ElementAt(target).Name} is **not** Compromised anymore.";
            } else
            {
                Participants.ElementAt(target).Strife += strife;
                res += $"{Participants.ElementAt(target).Name} is at {Participants.ElementAt(target).Strife} strife.";
            }
            return res;
        }

        public string ModifyFatigue(int userID, int target, int fatigue)
        {
            OrderByInit();
            using var ctx = new EmeraldBotContext();
            string res = "Fatigue has been applied.";
            if (target < 0 || target > Participants.Count - 1)
                throw new Exception($"The target number must be between 1 and {Participants.Count}.");

            Character c = Participants.ElementAt(target).Character;
            if (c is PC pc)
            {
                if (!ctx.CheckPrivateRights(Server.ID, userID, pc.ID)) throw new Exception("Only GMs or the character owner can modify its fatigue.");
                res += pc.ModifyFatigue(ctx, fatigue);
            }
            else if (c is NPC npc)
            {
                if (!ctx.CheckPrivateRights(Server.ID, userID, npc.ID)) throw new Exception("Only GMs can modify an NPC's strife.");

                int threshold = npc.Endurance;
                bool changed = false;
                if (fatigue > 0 && Participants.ElementAt(target).Fatigue <= threshold && Participants.ElementAt(target).Fatigue + fatigue > threshold) changed = true;
                else if (fatigue < 0 && Participants.ElementAt(target).Fatigue > threshold && Participants.ElementAt(target).Fatigue + fatigue <= threshold) changed = true;
                Participants.ElementAt(target).Fatigue += fatigue;
                if (changed && fatigue > 0) res += $" {Participants.ElementAt(target).Name} has become **Incapacitated**.";
                else if (changed && fatigue < 0) res += $" {Participants.ElementAt(target).Name} is **not** Incapacitated anymore.";
            }
            else
            {
                Participants.ElementAt(target).Fatigue += fatigue;
                res += $"{Participants.ElementAt(target).Name} is at {Participants.ElementAt(target).Fatigue} fatigue.";
            }
            return res;
        }
    }
}
