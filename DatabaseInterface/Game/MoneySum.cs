using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EmeraldBot.Model.Game
{
    public class MoneySum
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int Koku { get; set; } = 0;
        public int Bu { get; set; } = 0;
        public int Zeni { get; set; } = 0;
    }
}
