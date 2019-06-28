using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Characters
{
    public class Demeanor
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MinLength(5, ErrorMessage = "Name is too short"), MaxLength(64, ErrorMessage = "Name is too long")]
        [Required(ErrorMessage = "Can't create a demeanor without a name")]
        public string Name { get; set; } = "";

        [MinLength(5, ErrorMessage = "Effect is too short"), MaxLength(64, ErrorMessage = "Effect is too long")]
        [Required(ErrorMessage = "Can't create a demeanor without an effect")]
        public string Effect { get; set; } = "";

        [Required]
        [MinLength(4, ErrorMessage = "Unmasking is too short"), MaxLength(64, ErrorMessage = "Effect is too long")]
        public string Unmasking { get; set; } = "";

    }
}
