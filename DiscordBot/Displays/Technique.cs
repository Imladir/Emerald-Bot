using Discord;
using EmeraldBot.Bot.Tools;
using EmeraldBot.Model;
using EmeraldBot.Model.Game;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmeraldBot.Bot.Displays
{
    public static class TechniqueDisplay
    {
        public static List<Embed> GetProfile(this Technique t)
        {
            using var ctx = new EmeraldBotContext();
            var emd = new EmbedBuilder();

            t = ctx.Techniques.Find(t.ID);

            emd.WithTitle(t.Name);
            emd.WithColor(Colors.GetColor(t.Type.Name));
            emd.AddField("Type", t.Type.Name, true);
            if (t.Rank > 0) emd.AddField("Rank", t.Rank, true);
            emd.AddField("Ring", t.Ring == null ? "Any" : t.Ring.Name, true);

            List<Skill> skills = t.Skills.Select(x => x.Skill).ToList();
            foreach (var sg in t.SkillGroups.Select(x => x.SkillGroup)) skills.AddRange(ctx.Skills.Where(x => x.Group == sg).ToList());

            emd.AddField("Skill", String.Join("\n", skills.Select(s => s.Name)), true);

            if (t.TN != 0) emd.AddField("TN", t.TN, true);
            if (t.Source != null) emd.AddField("Source", $"{t.Source.Book}, p{t.Source.Page}", true);
            if (t.Activation != "") emd.AddField("Activation", t.Activation.ReplaceSymbols());
            if (t.Effect != "")
            {
                var fitLimit = t.Effect.ReplaceSymbols().FitDiscordMessageSize(1024);
                for (int i = 0; i < fitLimit.Count; i++) emd.AddField($"Effect {(i == 0 ? "" : " (continued)")}", fitLimit[i]);
            }
            return new List<Embed>() { emd.Build() };
        }
    }
}
