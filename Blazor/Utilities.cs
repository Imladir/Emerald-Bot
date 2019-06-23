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
