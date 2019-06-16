using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Blazor.Shared.Characters
{
    public class JournalEntry
    {
        public int ID { get; set; }
        public DateTime EntryDate { get; set; }
        public string Journal { get; set; }
        public int Amount { get; set; }
        public string Reason { get; set; }
    }
}
