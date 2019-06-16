using EmeraldBot.Model.Servers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Game
{
    public class Opportunity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public virtual Server Server { get; set; }

        public virtual Technique Source { get; set; }

        [Required]
        public bool Variable { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        [MaxLength(1024, ErrorMessage = "Effect is too long (must be shorter than 1024 characters)")]
        public string Effect { get; set; }

        [Required]
        public virtual ICollection<OpportunityTriggerOpportunity> Triggers { get; set; }

        public Opportunity()
        {
            Variable = false;
            Amount = 1;
        }

    }
}
