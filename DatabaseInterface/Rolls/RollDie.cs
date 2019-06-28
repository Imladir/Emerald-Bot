using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Rolls
{
    public class RollDie
    {
        public static Dictionary<string, List<DieFace>> Realisations;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; } = 0;

        public virtual DieFace Face { get; set; }

        public bool Exploded { get; set; } = false;

        public virtual Roll Roll { get; set; }

        public RollResult Score() { return Face.Score(); }
    }
}
