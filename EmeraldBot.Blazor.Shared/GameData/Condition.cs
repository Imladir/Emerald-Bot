using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Blazor.Shared.GameData
{
    public class Condition : NameAlias
    {
        public string Description { get; set; }

        public string Effect { get; set; }

        public string RemovedWhen { get; set; }
    }
}
