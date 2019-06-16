using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Bot.Displays
{
    public static class Colors
    {
        public static Color GetColor(string type)
        {
            switch (type)
            {
                case "Adversity": return Color.DarkRed;
                case "Anxiety": return Color.DarkOrange;
                case "Distinction": return Color.Teal;
                case "Passion": return Color.DarkTeal;
                default: return Color.Default;
            }
        }
    }
}
