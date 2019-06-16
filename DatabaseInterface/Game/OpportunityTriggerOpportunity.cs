using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Game
{
    public class OpportunityTriggerOpportunity
    {
        public int OpportunityID { get; set; }
        public virtual Opportunity Opportunity{ get; set; }

        public int OpportunityTriggerID { get; set; }
        public virtual OpportunityTrigger OpportunityTrigger { get; set; }
    }
}
