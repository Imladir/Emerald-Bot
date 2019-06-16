using EmeraldBot.Model.Characters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Servers
{
    public class DefaultCharacter
    {
        public int ServerID { get; set; }
        public virtual Server Server { get; set; }
        public int PlayerID { get; set; }

        public virtual Player Player { get; set; }
        public virtual Character Character { get; set; }
    }
}
