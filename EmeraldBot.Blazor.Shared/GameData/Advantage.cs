using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Blazor.Shared.GameData
{
    public class Advantage : NameAlias
    {
        public string AdvantageClass { get; set; }

        public string Ring { get; set; }

        public string PermanentEffect { get; set; }

        public string RollEffect { get; set; }
        public Tuple<string, int> Source { get; set; }

        public List<string> AdvantageTypes { get; set; }
    }
}