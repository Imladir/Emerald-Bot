using Discord;
using Discord.Commands;
using EmeraldBot.Bot.Tools;
using EmeraldBot.Model;
using EmeraldBot.Model.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Displays
{
    public static class Display
    {
        public static List<Embed> GetProfile(this PC pc, ulong serverID, ulong userID, ulong chanID, bool fullDetails = false)
        {
            var emds = new List<Embed>();

            using var ctx = new EmeraldBotContext();

            pc = ctx.PCs.Find(pc.ID);

            // Public data
            var emd = new EmbedBuilder();
            emd.WithTitle(pc.Name);
            emd.WithThumbnailUrl("https://gamepedia.cursecdn.com/l5r_gamepedia_en/thumb/1/1f/Rings.png/300px-Rings.png");
            if (pc.Icon != "") emd.WithThumbnailUrl(pc.Icon);

            if (pc.Age > 0) emd.AddField("Age", pc.Age);

            ctx.Entry(pc).Reference(x => x.Clan).Load();
            if (pc.Clan != null)
            {
                emd.WithThumbnailUrl(pc.Clan.Icon);
                emd.WithColor(new Color((uint)pc.Clan.Colour));
                emd.AddField("Clan", pc.Clan.Name, true);
            }
            if (pc.Icon != "") emd.WithThumbnailUrl(pc.Icon);
            if (pc.Family != "") emd.AddField("Family", pc.Family, true);
            if (pc.School != "") emd.AddField("School", pc.School, true);
            if (pc.Rank > 0) emd.AddField("Rank", pc.Rank, true);
            if (pc.Description != "") emd.AddField("Description", pc.Description);

            emds.Add(emd.Build());

            ctx.Entry(pc).Reference(x => x.Player).Load();
            if (ctx.CheckPrivateRights(serverID, userID, pc.ID) && ctx.CheckPrivateChannel(serverID, userID, chanID, (ulong)pc.Player.DiscordID))
            {
                emd = new EmbedBuilder();
                emd.WithColor(new Color((uint)pc.Clan.Colour));
                emd.WithTitle(String.Format("Private Data (alias: {0})", pc.Alias));
                if (pc.Ninjo != "") emd.AddField("Ninjo", pc.Ninjo);
                if (pc.Giri != "") emd.AddField("Giri", pc.Giri);

                string ringsStr = "";
                ctx.Entry(pc).Collection(x => x.Rings).Load();
                foreach (var r in pc.Rings)
                {
                    ctx.Entry(r).Reference(x => x.Ring).Load();
                    ringsStr += $"{r.Ring.Name}: {r.Value} {(r.Ring.Name.Equals("Void") ? $" (Current: {pc.CurrentVoid})" : "")}\n";
                }
                ringsStr += $"**Secondary Stats:**\nEndurance: {pc.Endurance}\n- Fatigue: {pc.Fatigue}\nComposure: {pc.Composure}\n- Strife: {pc.Strife}\n";
                ringsStr += $"Focus: {pc.Focus}\nVigilance: {pc.Vigilance}";
                emd.AddField("Rings", ringsStr, true);

                string skillsStr = "";
                var skillGroups = ctx.Entry(pc).Collection(x => x.Skills).Query().GroupBy(x => x.Skill.Group).OrderBy(x => x.Key.Name).ToList();
                foreach (var g in skillGroups)
                {
                    skillsStr += $"**{g.Key.Name}**\n";
                    foreach (var s in g)
                    {
                        ctx.Entry(s).Reference(x => x.Skill).Load();
                        skillsStr += $"- {s.Skill.Name}: {s.Value}\n";
                    }
                }
                if (skillsStr != "") emd.AddField("Skills", skillsStr, true);

                string techniquesStr = "";
                var techniqueType = ctx.Entry(pc).Collection(x => x.Techniques).Query().GroupBy(x => x.Technique.Type).OrderBy(x => x.Key.Name).ToList();
                foreach (var tt in techniqueType)
                {
                    techniquesStr += $"**{tt.Key.Name}**\n";
                    foreach (var t in tt)
                    {
                        techniquesStr += $"- {t.Technique.Name}\n";
                    }
                }
                if (techniquesStr != "") emd.AddField("Techniques", techniquesStr, true);

                string advantagesStr = "";
                var advantageClass = ctx.Entry(pc).Collection(x => x.Advantages).Query().GroupBy(x => x.Advantage.AdvantageClass).OrderBy(x => x.Key.Name).ToList();
                foreach (var ac in advantageClass)
                {
                    advantagesStr += $"**{ac.Key.Name}**\n";
                    foreach (var a in ac)
                    {
                        advantagesStr += $"- {a.Advantage.Name}\n";
                    }
                }
                if (advantagesStr != "") emd.AddField("Techniques", advantagesStr, true);

                ctx.Entry(pc).Collection(x => x.JournalEntries).Load();
                emd.AddField("Journal", $"**Glory**: {pc.Glory}\n**Status**: {pc.Status}\n" +
                                $"**Honour**: {pc.Honour}\n**XP**: {pc.XP}\n", true);
                emds.Add(emd.Build());
            }

            return emds;
        }

        public static List<Embed> GetProfile(this NPC npc, ulong serverID, ulong userID, ulong chanID, bool fullDetails = false)
        {
            throw new NotImplementedException("Not done yet");
        }
    }
}
