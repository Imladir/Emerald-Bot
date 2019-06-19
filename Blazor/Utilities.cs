using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor
{
    public static class Utilities
    {
        private static string speechPattern = "((\".*?\")|(“.*?”))";
        private static string thoughtsPattern = "\\~(.*?)\\~";
        public static string ToHTML(this string s)
        {
            s = s.Replace("\n", "<br />\n");
            return SimpleFormat(s);
        }

        public static string SimpleFormat(String msg)
        {
            msg = Regex.Replace(msg, speechPattern, SimpleSpeechFormater);
            msg = Regex.Replace(msg, thoughtsPattern, SimpleThoughtsFormater);

            return msg;
        }

        private static string SimpleSpeechFormater(Match match)
        {
            return $"<span style=\"font-weight: bold; \">{match.Value}</span>";
        }

        private static string SimpleThoughtsFormater(Match match)
        {
            return String.Format("<em>{0}</em>", match.Groups[1].Value);
        }
    }
}
