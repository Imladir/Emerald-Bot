using EmeraldBot.Model.Characters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EmeraldBot.Model.Scenes
{
    public class ConflictParticipant
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public virtual Character Character { get; set; }
        public virtual Conflict Conflict { get; set; }
        [MaxLength(128)]
        public string Name { get; set; } = "";
        public bool IsLate { get; set; } = false;
        public int Init { get; set; } = 0;
        public int Fatigue { get; set; } = 0;
        public int Strife { get; set; } = 0;
    }
}
