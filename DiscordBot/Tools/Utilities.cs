using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Discord;
using EmeraldBot.Model;
using EmeraldBot.Model.Servers;

namespace EmeraldBot.Bot.Tools
{
    public class PrivateCharacter {
        public Model.Characters.Character Character { get; set; }
    }
    public class GeneralCharacter {
        public Model.Characters.Character Character { get; set; }
    }
    public static class Utilities {

        public static string Capitalize(this string s)
        {
            return s.First().ToString().ToUpper() + s.Substring(1).ToLower();
        }

        public static string ReplaceSymbols(this string data, ulong serverID = 0)
        {
            using (EmeraldBotContext context = new EmeraldBotContext())
            {
                foreach (var emote in context.Emotes.Where(x => x.Server.DiscordID == (long)serverID || x.Server.DiscordID == 0))
                {
                    data = data.Replace(@"{" + emote.Code + @"}", emote.Text);
                }
            }
            return data;
        }

        public static List<string> FitDiscordMessageSize(this string msg, int limit = 2000)
        {
            return new List<string>() { msg }.FitDiscordMessageSize(limit);
        }

        public static List<string> FitDiscordMessageSize(this List<string> msg, int limit = 2000)
        {
            string splitPattern = @"(?<data>.{1," + limit + @"}(?=[\s]))";
            List<string> res = new List<string>();
            string current = "";
            foreach(var s in msg)
            {
                if (current.Length + s.Length < limit) current += $"{s}\n";
                else
                {
                    res.Add(current);
                    if (s.Length < limit) current = $"{s}\n";
                    else
                    {
                        var splits = s.Split('\n').ToList();
                        if (splits.Count < 2)
                            splits = Regex.Split(s, splitPattern).ToList();
                        res.AddRange(splits.FitDiscordMessageSize(limit));
                        current = "";
                    }
                }
            }
            if (current != "") res.Add(current);

            return res;
        }

        public static Embed ToEmbed(this Message msg)
        {
            var emd = new EmbedBuilder();
            emd.WithTitle(msg.Title);
            emd.WithColor(new Color((uint)msg.Colour));
            emd.WithThumbnailUrl(msg.Icon);
            emd.WithDescription(AutoFormater.SimpleFormat(msg.Text));
            return emd.Build();
        }
    }
}
