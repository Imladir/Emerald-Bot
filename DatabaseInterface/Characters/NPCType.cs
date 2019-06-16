using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Characters
{
    public class NPCType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MinLength(5, ErrorMessage = "Name is too short"), MaxLength(64, ErrorMessage = "Name is too long")]
        [Required(ErrorMessage = "Can't create an NPC type without a name")]
        public string Name { get; set; }
    }
}
