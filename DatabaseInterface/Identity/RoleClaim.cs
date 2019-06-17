using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Text;

namespace EmeraldBot.Model.Identity
{
    public class RoleClaim
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public virtual Role Role { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public Claim ToClaim()
        {
            return new Claim(ClaimType, ClaimValue);
        }
        public virtual void InitializeFromClaim(Claim other)
        {
            ClaimType = other?.Type;
            ClaimValue = other?.Value;
        }
    }
}
