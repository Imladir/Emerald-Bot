using EmeraldBot.Model.Servers;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Identity
{
    public class UserRole
    {
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public int RoleID { get; set; }
        public virtual Role Role { get; set; }
        public virtual Server Server { get; set; }
    }
}
