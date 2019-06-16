using EmeraldBot.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmeraldBot.Blazor.Server
{
    public static class Converters
    {
        public static Shared.GameData.Clan ToBlazor(this Model.Game.Clan clan)
        {
            var res = new Shared.GameData.Clan()
            {
                ID = clan.ID,
                Name = clan.Name,
                Icon = clan.Icon,
                Colour = clan.Colour
            };
            return res;
        }

        public static Shared.Characters.PC ToBlazor(this Model.Characters.PC pc)
        {
            using var ctx = new EmeraldBotContext();
            var queryClan = from c in ctx.Clans
                        join na in ctx.PCs on c.ID equals na.Clan.ID
                        where na.ID == pc.ID
                        select c;
            var queryPlayer = from p in ctx.Players
                              join na in ctx.PCs on p.ID equals na.Player.ID
                              where na.ID == pc.ID
                              select p;

            var res = new Shared.Characters.PC()
            {
                Age = pc.Age,
                Alias = pc.Alias,
                Clan = queryClan.Single().ToBlazor(),
                Composure = pc.Composure,
                CurrentVoid = pc.CurrentVoid,
                Description = pc.Description,
                Endurance = pc.Endurance,
                Family = pc.Family,
                Fatigue = pc.Fatigue,
                Focus = pc.Focus,
                Giri = pc.Giri,
                Icon = pc.Icon,
                ID = pc.ID,
                Name = pc.Name,
                Ninjo = pc.Ninjo,
                Player = queryPlayer.Single().ToBlazor(),
                Rank = pc.Rank,
                School = pc.School,
                Strife = pc.Strife
            };
            return res;
        }

        public static Shared.Message ToBlazor(this Model.Servers.Message msg)
        {
            dynamic data = JObject.Parse(msg.Data);
            var res = new Shared.Message()
            {
                ChannelID = msg.DiscordChannelID,
                DiscordID = msg.DiscordMessageID,
                CreatedDate = msg.CreatedDate,
                CharacterID = msg.Character.ID,
                ID = msg.ID,
                ServerID = msg.Server.ID,
                Text = data.Description,
                Title = data.Title
            };
            return res;
        }

        public static Shared.Player ToBlazor(this Model.Servers.Player player)
        {
            using var ctx = new EmeraldBotContext();
            var res = new Shared.Player()
            {
                DiscordID = player.DiscordID,
                ID = player.ID,
                IsAdmin = player.ID == 1, // CHANGE THAT
                Name = player.Name
            };

            res.IsGmOn = ctx.Servers.Where(x => x.GMs.Any(y => y.PlayerID == player.ID)).Select(x => x.ID).ToList();

            return res;
        }
    }
}
