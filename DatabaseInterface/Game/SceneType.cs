﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Game
{
    public class SceneType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(64, ErrorMessage = "Name is too long")]
        [Required(ErrorMessage = "Can't create a scene type without a name")]
        public string Name { get; set; }

        public virtual ICollection<ActionScenes> Actions { get; set; }
    }
}
