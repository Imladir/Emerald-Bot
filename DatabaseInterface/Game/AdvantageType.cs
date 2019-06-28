using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Game
{
    public class AdvantageType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MinLength(3, ErrorMessage = "Advantage Type name can't be less than 3 characters")]
        [MaxLength(16, ErrorMessage = "Advantage Type name can't be longer than 8 characters")]
        public string Name { get; set; } = "";
        public virtual ICollection<AdvantageTypeAdvantages> Advantages { get; set; }
    }
}
