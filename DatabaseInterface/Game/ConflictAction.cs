using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Game
{
    public class ConflictAction
    {
        public int ConflictID { get; set; }
        public virtual ConflictType Conflict { get; set; }

        public int ActionID { get; set; }
        public virtual Action Action { get; set; }
    }
}
