using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EmeraldBot.Model.Game
{
    public class GearRarity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int MinRarity { get; set; } = 1;
        public int MaxRarity { get; set; } = 1;
    }
}
