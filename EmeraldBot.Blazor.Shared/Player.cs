using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Blazor.Shared
{
    public class Player
    {
        public int ID { get; set; }
        public long DiscordID { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public List<int> IsGmOn { get; set; }
    }
}
