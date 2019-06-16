using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmeraldBot.Model.Game
{
    public class Action
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(16, ErrorMessage = "Action name can't be longer than 16 characters")]
        [MinLength(4, ErrorMessage = "Action name can't be less than 5 characters")]
        public string Name { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }
        [MaxLength(1024)]
        public string Activation { get; set; }
        [MaxLength(1024)]
        public string Effect { get; set; }
        public virtual ICollection<ConflictAction> Conflicts { get; set; }
        public virtual ICollection<ActionTypeAction> Types { get; set; }

        public Action()
        {
            Conflicts = new List<ConflictAction>();
            Types = new List<ActionTypeAction>();
        }
    }
}
