using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Characters
{
    public class PCCondition
    {
        public int PCID { get; set; }
        public virtual PC PC { get; set; }

        public int ConditionID { get; set; }
        public virtual Condition Condition { get; set; }
    }
}
