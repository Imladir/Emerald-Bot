using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Blazor.Shared.GameData
{
    public class Technique : NameAlias
    {
        public string Type { get; set; }
        public int Rank { get; set; }
        public string Ring { get; set; }
        public int TN { get; set; }
        public Tuple<string, int> Source { get; set; }
        public string Activation { get; set; }
        public string Effect { get; set; }

        public List<SkillGroup> SkillGroups { get; set; }
        public List<Skill> Skills { get; set; }
    }
}