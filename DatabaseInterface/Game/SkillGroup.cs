using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Game
{
    public class SkillGroup
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required(ErrorMessage = "A skill group must have a name")]
        [RegularExpression(@"^\w{4,16}$", ErrorMessage = "Skill group name must be one word of length 4 to 16")]
        public string Name { get; set; }

        public virtual ICollection<TechniqueSkillGroup> Techniques { get; set; }
    }
}
