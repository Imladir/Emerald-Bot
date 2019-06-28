using EmeraldBot.Model.Rolls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Servers
{
    public class Emote
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public virtual Server Server { get; set; }

        [Required]
        public string Code { get; set; } = "";

        [Required]
        [MaxLength(64, ErrorMessage = "Emote text is too long")]
        public string Text { get; set; } = "";

        public virtual ICollection<DieFace> DieFaces { get; set; }
    }
}
