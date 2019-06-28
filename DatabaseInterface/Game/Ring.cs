using EmeraldBot.Model.Characters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Game
{
    public class Ring
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MinLength(3, ErrorMessage = "Ring name can't be less than 3 characters")]
        [MaxLength(16, ErrorMessage = "Ring name can't be longer than 8 characters")]
        public string Name { get; set; } = "";

        public virtual ICollection<CharacterRing> Characters { get; set; }
        public virtual ICollection<Technique> Techniques { get; set; }
        public virtual ICollection<Advantage> Advantages { get; set; }
        public virtual ICollection<Opportunity> Opportunities { get; set; }
    }
}
