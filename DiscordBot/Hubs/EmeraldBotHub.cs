﻿using Discord.WebSocket;
using EmeraldBot.Bot.Tools;
using EmeraldBot.Model;
using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Identity;
using EmeraldBot.Model.Servers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Hubs
{
    [AllowAnonymous]
    public class EmeraldBotHub : Hub
    {
        public string GreetAll()
        {
            Console.WriteLine("Received a greet all message");
            return "Server says hi";
        }

        public async Task GetUserGuilds(int id)
        {
            using (var ctx = new EmeraldBotContext())
            {
                var player = ctx.Users.Find(id);
                // Retrieve the player's characters
                var query =
                from servers in ctx.Servers
                join pcs in ctx.Characters.OfType<PC>() on servers.ID equals pcs.Server.ID
                join p in ctx.Users on pcs.Player.ID equals p.ID
                where p.ID == player.ID
                select new { servers.ID, servers.Name };
                var res = query.ToDictionary(x => x.ID, x => x.Name);
                await Clients.Caller.SendAsync("UserGuilds", player.ID, res);
            }
        }

        public async Task GetUserGuildChannels(int userID, int serverID)
        {
            using (var ctx = new EmeraldBotContext())
            {
                var server = ctx.Servers.Find(serverID);
                var player = ctx.Users.Find(userID);
                var discordUser = WebServiceServer.DiscordClient.GetUser((ulong)player.DiscordID);
                var discordGuild = WebServiceServer.DiscordClient.GetGuild((ulong)server.DiscordID);
                var channels = discordGuild.Channels.ToList().FindAll(x => x.Users.Contains(discordUser));
                var res = channels.ToDictionary(x => x.Id, x => x.Name);
                await Clients.Caller.SendAsync("UserChannels", userID, res);
            }
        }

        public async Task GetUserGuildCharacters(int userID, int serverID)
        {
            using (var ctx = new EmeraldBotContext())
            {
                var pcs = ctx.Characters.OfType<PC>().Where(x => x.Server.ID == serverID
                                                              && x.Player.ID == userID);
                var res = pcs.ToDictionary(x => x.ID, x => x.Name);
                await Clients.Caller.SendAsync("UserCharacters", userID, res);
            }
        }

        public async Task<int> SendMessage(string title, string text, int serverID, long channelID, int playerID, int characterID)
        {
            using var ctx = new EmeraldBotContext();
            var target = ctx.PCs.Find(characterID);
            var emd = AutoFormater.Format(target, text);
            emd.Title += title;

            var channel = WebServiceServer.DiscordClient.GetChannel((ulong)channelID) as ISocketMessageChannel;
            var res = await channel.SendMessageAsync("", false, emd.Build());

            var server = ctx.Servers.Find(serverID);
            var player = ctx.Users.Find(playerID);
            ctx.Entry(player).Collection(x => x.Messages).Load();

            var message = new Message()
            {
                Data = JsonConvert.SerializeObject(emd),
                DiscordChannelID = (long)channelID,
                DiscordMessageID = (long)res.Id,
                LastUpdated = DateTime.UtcNow,
                Server = server,
                Player = player,
            };
            if (characterID > 0) message.Character = ctx.PCs.Find(characterID);
            ctx.Messages.Add(message);
            ctx.SaveChanges();

            return message.ID;
        }

        /******************
         * Overloads
         *****************/
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Received a connection request from client");
            Clients.Caller.SendAsync("Connected", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("Client disconnected");
            return base.OnDisconnectedAsync(exception);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
