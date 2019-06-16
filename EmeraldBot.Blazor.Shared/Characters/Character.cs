using EmeraldBot.Blazor.Shared.GameData;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Blazor.Shared.Characters
{
    public abstract class Character : NameAlias
    {
        public string Icon { get; set; }
        public string Description { get; set; }
        public int Fatigue { get; set; }
        public int Strife { get; set; }
        public int Endurance { get; set; }
        public int Composure { get; set; }
        public int Focus { get; set; }
        public int Vigilance { get; set; }
        public virtual Dictionary<string, int> Rings { get; set; } = new Dictionary<string, int>();
        public virtual List<Condition> Conditions { get; set; } = new List<Condition>();

        public Character()
        {
            Icon = "";
            Description = "";
            Fatigue = 0;
            Strife = 0;
        }
    }
}
