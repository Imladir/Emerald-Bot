using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Scenes
{
    public class ConflictType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(64, ErrorMessage = "Name is too long")]
        [Required(ErrorMessage = "Can't create a conflict type without a name")]
        public string Name { get; set; } = "";

        [Required]
        [MaxLength(1024)]
        public string Description { get; set; } = "";
        public virtual ICollection<ConflictAction> Actions { get; set; }
    }
}
