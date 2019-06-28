using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmeraldBot.Model.Scenes
{
    public class ActionType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(16)]
        public string Name { get; set; } = "";

        [MaxLength(1024)]
        public string Description { get; set; } = "";
        public virtual ICollection<ActionTypeAction> Actions { get; set; }
    }
}
