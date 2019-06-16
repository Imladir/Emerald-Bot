using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Blazor.Shared
{
    public abstract class NameAlias
    {
        public int ID { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; }
    }
}
