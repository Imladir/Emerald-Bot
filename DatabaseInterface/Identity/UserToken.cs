using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EmeraldBot.Model.Identity
{
    public class UserToken
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public virtual User User { get; set; }
        public string Name { get; set; }

        [ProtectedPersonalData]
        public string Value { get; set; }
    }
}
