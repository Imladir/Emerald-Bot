using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Blazor.Shared
{
    public class Message
    {
        public int ID { get; set; }
        public long DiscordID { get; set; }
        public int ServerID { get; set; }
        public int CharacterID { get; set; }
        public long ChannelID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }

        public Message()
        {
            ID = 0;
            DiscordID = 0;
            ServerID = 0;
            CharacterID = 0;
            ChannelID = 0;
            Title = "";
            Text = "";
            CreatedDate = DateTime.UtcNow;
        }
    }
}
