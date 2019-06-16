using Discord;
using EmeraldBot.Bot.Tools;
using EmeraldBot.Model;
using EmeraldBot.Model.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmeraldBot.Bot.Displays
{
    public static class AdvantageDisplay
    {
        public static List<Embed> GetProfile(this Advantage a)
        {
            var emd = new EmbedBuilder();

            using var ctx = new EmeraldBotContext();

            a = ctx.Advantages.Find(a.ID);

            emd.WithTitle($"{a.Name}\n{a.AdvantageClass.Name}");
            emd.WithColor(Colors.GetColor(a.AdvantageClass.Name));
            if (a.AdvantageTypes.Count > 0) emd.AddField("Type", String.Join(", ", a.AdvantageTypes.Select(x => x.AdvantageType.Name)));
            if (a.Ring != null) emd.AddField("Ring", a.Ring == null ? "Any" : a.Ring.Name, true);
            if (a.PermanentEffect != "") emd.AddField("Permanent Effect", a.PermanentEffect.ReplaceSymbols());
            if (a.RollEffect != "") emd.AddField("Roll Effect", a.RollEffect.ReplaceSymbols());

            return new List<Embed>() { emd.Build() };
        }
    }
}
