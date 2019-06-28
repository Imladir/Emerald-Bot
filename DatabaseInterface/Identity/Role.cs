﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Text;

namespace EmeraldBot.Model.Identity
{
    public class Role
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; } = "";
        public string NormalizedRoleName { get; set; } = "";
        public string Description { get; internal set; } = "";
        public virtual DateTime LastUpdate { get; set; } = DateTime.UtcNow;
        public virtual ICollection<UserRole> Users { get; set; } = new List<UserRole>();
        public virtual ICollection<RoleClaim> Claims { get; set; } = new List<RoleClaim>();
    }
}
