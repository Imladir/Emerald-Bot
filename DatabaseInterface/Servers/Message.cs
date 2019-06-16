using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Servers;
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
        public virtual Server Server { get; set; }
        public virtual Player Player { get; set; }
        public virtual PC Character { get; set; }
        public long DiscordChannelID { get; set; }
        public long DiscordMessageID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Data { get; set; }

        public Message()
        {
            CreatedDate = DateTime.UtcNow;
            LastUpdated = DateTime.UtcNow;
        }
    }
}
