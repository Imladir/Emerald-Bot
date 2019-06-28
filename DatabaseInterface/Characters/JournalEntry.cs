using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Characters
{
    public class JournalEntry
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public DateTime EntryDate { get; set; }
        [Required]
        public virtual JournalType Journal { get; set; }
        [Required]
        public int Amount { get; set; } = 0;
        [Required]
        [MaxLength(1024, ErrorMessage = "Reason is too long (must be less than 1024 characters")]
        public string Reason { get; set; } = "";
        public bool IsCurriculum { get; set; } = false;

        public JournalEntry()
        {
            EntryDate = DateTime.UtcNow;
        }
    }
}
