using EmeraldBot.Model.Servers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EmeraldBot.Model.Servers
{

    [Table("GMs")]
    public class GM
    {
        public int ServerID { get; set; }
        public virtual Server Server { get; set; }

        public int PlayerID { get; set; }
        public virtual Player Player { get; set; }
    }
}
