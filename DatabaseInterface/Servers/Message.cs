using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Servers
{
    public class Message
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column(Order = 0)]
        public int ID { get; set; }
        public virtual Server Server { get; set; } = null;
        public virtual User Player { get; set; } = null;
        public long DiscordChannelID { get; set; } = 0;
        public long DiscordMessageID { get; set; } = 0;
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Title { get; set; } = "";
        public string Text { get; set; } = "";
        public string Icon { get; set; } = "";
        public int Colour { get; set; } = 0;

        public Message()
        {
            CreatedDate = DateTime.UtcNow;
            LastUpdated = DateTime.UtcNow;
        }
    }
}
