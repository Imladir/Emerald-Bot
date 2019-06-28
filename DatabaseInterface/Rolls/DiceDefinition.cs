using EmeraldBot.Model.Servers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Rolls
{
    public class DieFace
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string DieType { get; set; } = "";

        [Required]
        [MaxLength(2, ErrorMessage = "Value is too long")]
        //[Required(ErrorMessage = "Can't create a die side without a value")]
        public string Value { get; set; } = "";

        [Required]
        public virtual Emote Emote { get; set; }

        public bool Success() { return Value.Contains("s"); }
        public bool ExplosiveSuccess() { return Value.Contains("e"); }
        public bool Opportunity() { return Value.Contains("o"); }
        public bool Strife() { return Value.Contains("t"); }
    }

    public static class DieDefinitionMethods
    {
        public static RollResult Score(this DieFace d) { return new RollResult(d); }
    }
}
