using Discord;
using Discord.WebSocket;
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
using System.Text.RegularExpressions;
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

        public List<int> GetUserGuilds(int userID)
        {
            try
            {
                using var ctx = new EmeraldBotContext();

                var player = ctx.Users.Find(userID);
                var user = WebServiceServer.DiscordClient.GetUser((ulong)player.DiscordID) as SocketUser;
                var guildList = user.MutualGuilds;

                var res = guildList.OrderBy(x => x.Name).Select(x => ctx.Servers.Single(y => (ulong)y.DiscordID == x.Id).ID).ToList();
                return res;
            } catch (Exception e)
            {
                Console.WriteLine($"Failed to retrieve guild list: {e.Message}\n{e.StackTrace}");
                return new List<int>();
            }
        }

        public Dictionary<ulong, string> GetUserGuildChannels(int userID, int serverID)
        {
            using var ctx = new EmeraldBotContext();
            var server = ctx.Servers.Find(serverID);
            var player = ctx.Users.Find(userID);
            var discordUser = WebServiceServer.DiscordClient.GetUser((ulong)player.DiscordID);
            var discordGuild = WebServiceServer.DiscordClient.GetGuild((ulong)server.DiscordID);
            var channels = discordGuild.Channels.ToList().FindAll(x => x.Users.Any(x => x.Id == discordUser.Id) && x is SocketTextChannel);
            var res = channels.ToDictionary(x => x.Id, x => x.Name);
            return res;
        }

        public async Task SendMessage(int messageID)
        {
            using var ctx = new EmeraldBotContext();
            var message = ctx.Messages.Find(messageID);

            var channel = WebServiceServer.DiscordClient.GetChannel((ulong)message.DiscordChannelID) as ISocketMessageChannel;

            if (message.DiscordMessageID == 0)
            {
                var res = await channel.SendMessageAsync("", false, message.ToEmbed());
                message.DiscordMessageID = (long)res.Id;
                ctx.SaveChanges();
            } else
            {
                IUserMessage msg = (IUserMessage)await channel.GetMessageAsync((ulong)message.DiscordMessageID);
                await msg.ModifyAsync(x => x.Embed = message.ToEmbed());
            }
        }

        public async Task DeleteMessage(int messageID)
        {
            using var ctx = new EmeraldBotContext();
            var message = ctx.Messages.Find(messageID);

            var channel = WebServiceServer.DiscordClient.GetChannel((ulong)message.DiscordChannelID) as ISocketMessageChannel;
            IUserMessage msg = (IUserMessage)await channel.GetMessageAsync((ulong)message.DiscordMessageID);
            await msg.DeleteAsync();
            ctx.Messages.Remove(message);
            ctx.SaveChanges();
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
