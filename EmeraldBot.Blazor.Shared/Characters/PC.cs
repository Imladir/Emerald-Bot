using EmeraldBot.Blazor.Shared.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmeraldBot.Blazor.Shared.Characters
{
    public class PC : Character
    {
        public Player Player { get; set; }
        public Clan Clan { get; set; }
        public string Family { get; set; }
        public string School { get; set; }
        public int Rank { get; set; }
        public int Age { get; set; }
        public string Ninjo { get; set; }
        public string Giri { get; set; }
        public int XP { get { return CurrentJournalValue("XP"); } }

        public int Honour { get { return CurrentJournalValue("Honour"); } }

        public int Glory { get { return CurrentJournalValue("Glory"); } }

        public int Status { get { return CurrentJournalValue("Status"); } }

        public int CurrentVoid { get; set; }

        public List<Advantage> Advantages { get; set; } = new List<Advantage>();
        public List<Technique> Techniques { get; set; } = new List<Technique>();
        public List<Skill> Skills { get; set; } = new List<Skill>();
        public List<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();

        public int CurrentJournalValue(string type)
        {
            return 0; // JournalEntries.Where(x => x.Journal.Equals(type, StringComparison.OrdinalIgnoreCase)).Sum(x => x.Amount);
        }
    }
}