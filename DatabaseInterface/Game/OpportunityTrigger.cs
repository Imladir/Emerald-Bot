using EmeraldBot.Model.Scenes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Game
{
    public class OpportunityTrigger
    { 
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public virtual Ring Ring { get; set; }
        public virtual SkillGroup SkillGroup { get; set; }
        public virtual TechniqueType TechniqueType { get; set; }
        public virtual Technique Technique { get; set; }
        public virtual SceneType SceneType { get; set; }
        public virtual Scenes.Action Action { get; set; }
        public virtual ICollection<OpportunityTriggerOpportunity> Opportunities { get; set; }
    }
}
