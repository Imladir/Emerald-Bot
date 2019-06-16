using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Game
{
    public class AdvantageTypeAdvantages
    {
        public int AdvantageTypeID { get; set; }
        public virtual AdvantageType AdvantageType { get; set; }

        public int AdvantageID { get; set; }
        public virtual Advantage Advantage { get; set; }
    }
}
