using EmeraldBot.Model.Game;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor
{
    public static class Utilities
    {
        public static string GetImage(this Ring r)
        {
            return $"img/{r.Name}.png";
        }
        public static string Capitalize(this string s)
        {
            return s.First().ToString().ToUpper() + s.Substring(1).ToLower();
        }

        public static string GetString(this WeaponGrip g)
        {
            string res = "";
            if (g.DamageModificator > 0) res += $"Damage +{g.DamageModificator} ";
            else if (g.DamageModificator < 0) res += $"Damage -{g.DamageModificator} ";

            if (g.DeadlinessModificator > 0) res += $"Deadliness +{g.DeadlinessModificator} ";
            else if (g.DeadlinessModificator < 0) res += $"Deadliness -{g.DeadlinessModificator} ";

            if (g.NewRangeMin != -1 && g.NewRangeMax != -1)
            {
                if (g.NewRangeMin == g.NewRangeMax) res += $"Range: {g.NewRangeMin}";
                else res += $"Range: {g.NewRangeMin}-{g.NewRangeMax}";
            }
            if (res == "") res = g.Hands == 1 ? "1 hand: standard profile" : "2 hands: standard profile";
            else res = (g.Hands == 1 ? "1 hand: " : "2 hands: ") + res;
            return res;
        }

        public static string GetString(this MoneySum cost)
        {
            List<string> res = new List<string>();
            if (cost.Koku > 0) res.Add($"{cost.Koku} Koku");
            if (cost.Bu > 0) res.Add($"{cost.Bu} Bu");
            if (cost.Zeni > 0) res.Add($"{cost.Zeni} Zeni");
            return string.Join(", ", res);
        }

        private static string boldPattern = "((\".*?\")|(“.*?”)|(\\*\\*.*?\\*\\*))";
        private static string ItalicPattern = @"[\*~](.*?)[\*~]";
        public static string ToHTML(this string s)
        {
            s = WebUtility.HtmlEncode(s).Replace("&quot;", "\"");
            s = s.Replace("\n", "<br />\n");
            return SimpleFormat(s);
        }

        public static string SimpleFormat(String msg)
        {
            msg = Regex.Replace(msg, boldPattern, BoldFormatter);
            msg = Regex.Replace(msg, ItalicPattern, ItalicFormater);

            return msg;
        }

        public static string ReplaceSymbols(this string s)
        {
            s = WebUtility.HtmlEncode(s);
            s = Regex.Replace(s, boldPattern, BoldFormatter).Replace("**", "");
            s = s.Replace("{r}", "<img src=\"img/black.gif\" style=\"width: 20px\" />");
            s = s.Replace("{r-}", "<img src=\"img/black.png\" alt=\"Ring die\" style=\"width: 20px\" />");
            s = s.Replace("{ret}", "<img src=\"img/blacket.png\" style=\"width: 20px\" />");
            s = s.Replace("{ro}", "<img src=\"img/blacko.png\" style=\"width: 20px\" />");
            s = s.Replace("{rot}", "<img src=\"img/blackot.png\" style=\"width: 20px\" />");
            s = s.Replace("{rs}", "<img src=\"img/blacks.png\" style=\"width: 20px\" />");
            s = s.Replace("{rst}", "<img src=\"img/blackst.png\" style=\"width: 20px\" />");

            s = s.Replace("{s}", "<img src=\"img/white.gif\" style=\"width: 20px\" />");
            s = s.Replace("{s-}", "<img src=\"img/white.png\" alt=\"Skill die\" style=\"width: 20px\" />");
            s = s.Replace("{se}", "<img src=\"img/white.png\" style=\"width: 20px\" />");
            s = s.Replace("{set}", "<img src=\"img/white.png\" style=\"width: 20px\" />");
            s = s.Replace("{so}", "<img src=\"img/white.png\" style=\"width: 20px\" />");
            s = s.Replace("{ss}", "<img src=\"img/white.png\" style=\"width: 20px\" />");
            s = s.Replace("{sso}", "<img src=\"img/white.png\" style=\"width: 20px\" />");
            s = s.Replace("{sst}", "<img src=\"img/white.png\" style=\"width: 20px\" />");

            s = s.Replace("{success}", "<img src=\"img/success.png\" alt=\"Success\" style=\"width: 20px\" />");
            s = s.Replace("{opportunity}", "<img src=\"img/opportunity.png\" alt=\"Opportunity\" style=\"width: 20px\" />");
            s = s.Replace("{strife}", "<img src=\"img/strife.png\" alt=\"Strife\" style=\"width: 20px\" />");
            s = s.Replace("{explosive}", "<img src=\"img/explosive.png\" alt=\"Explosive Success\" style=\"width: 20px\" />");

            s = s.Replace("\n", "<br />\n");
            return s;
        }

        private static string BoldFormatter(Match match)
        {
            return $"<span style=\"font-weight: bold; \">{match.Value}</span>";
        }

        private static string ItalicFormater(Match match)
        {
            return String.Format("<em>{0}</em>", match.Groups[1].Value);
        }
    }
}
