using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Blazor.Shared.GameData
{
    public class Skill : NameAlias
    {
        public SkillGroup Group { get; set; }

        public Tuple<string, int> Source { get; set; }
    }
}