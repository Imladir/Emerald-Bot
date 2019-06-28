using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Game
{
    public class AdvantageClass
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MinLength(3, ErrorMessage = "Advantage Class name can't be less than 3 characters")]
        [MaxLength(16, ErrorMessage = "Advantage Class name can't be longer than 8 characters")]
        public string Name { get; set; } = "";
    }
}
