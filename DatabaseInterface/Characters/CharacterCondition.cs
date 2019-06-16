using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Characters
{
    public class CharacterCondition
    {
        public int CharacterID { get; set; }
        public virtual Character Character { get; set; }

        public int ConditionID { get; set; }
        public virtual Condition Condition { get; set; }
    }
}
