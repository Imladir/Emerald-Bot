using Discord;
using Discord.Commands;
using EmeraldBot.Bot.Tools;
using EmeraldBot.Model;
using EmeraldBot.Model.Identity;
using EmeraldBot.Model.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Modules
{
    [Group("admin")]
    public class Admin : ModuleBase<SocketCommandContext>
    {
        [Group("gm")]
        public class GmModule : ModuleBase<SocketCommandContext>
        {
            [RequireOwner(Group = "Permission")]
            [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
            [Command("add")]
            [Alias("a")]
            [Summary("Gives an user GM rights over the bot. Only the server's owner can use that command.")]
            public async Task AddGM([Summary("User to grant GM rights to.")]
                                    IUser user)
            {
                try
                {
                    var newGM = user as IGuildUser;

                    using var ctx = new EmeraldBotContext();
                    var server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id);
                    if (server.IsGM(ctx, newGM.Id)) throw new Exception($"{newGM.Nickname} is already a GM on the server.");

                    var player = ctx.Users.SingleOrDefault(x => x.DiscordID == (long)newGM.Id);
                    if (player == null)
                        player = new User() { DiscordID = (long)newGM.Id };

                    player.Roles.Add(new UserRole() { User = player, Role = ctx.Roles.Single(x => x.Name.Equals("GM")), Server = server });
                    player.Claims.Add(new UserClaim() { User = player, ClaimType = $"Server-{server.DiscordID}", ClaimValue = "GM" });
                    ctx.SaveChanges();
                    await ReplyAsync($"{newGM.Nickname} is now a GM on the server.");

                }
                catch (Exception e)
                {
                    await ReplyAsync(e.Message);
                    Console.WriteLine($"Exception: {e.Message}:\n{e.StackTrace}");
                }
            }

            [RequireOwner(Group = "Permission")]
            [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
            [Command("remove")]
            [Alias("r")]
            [Summary("Removes an user's GM rights over the bot. Only the server's owner can use that command.")]
            public async Task RemoveGM([Summary("User to remove GM rights to.")]
                                    IUser user)
            {
                try
                {
                    var gm = user as IGuildUser;

                    using (var ctx = new EmeraldBotContext())
                    {
                        var server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id);
                        if (!server.IsGM(ctx, gm.Id)) throw new Exception($"{gm.Nickname} is not a GM on the server.");

                        var player = ctx.Users.SingleOrDefault(x => x.DiscordID == (long)gm.Id);

                        var res = ctx.UserRoles.Single(x => x.Role == ctx.Roles.Single(x => x.Name.Equals("GM"))
                                                         && x.User.ID == player.ID
                                                         && x.Server.ID == server.ID);
                        player.Roles.Remove(res);
                        var claim = player.Claims.Single(x => x.ClaimType.Equals($"Server-{server.DiscordID}") && x.ClaimValue.Equals(res.Role.Name));
                        player.Claims.Remove(claim);
                        ctx.SaveChanges();
                        await ReplyAsync($"{gm.Nickname} is no longer a GM on the server.");
                    }
                }
                catch (Exception e)
                {
                    await ReplyAsync(e.Message);
                    Console.WriteLine($"Exception: {e.Message}:\n{e.StackTrace}");
                }
            }
        }

        [RequireOwner(Group = "Permission")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [Command("setprefix")]
        [Summary("Changes the prefix for the bot's commands")]
        public async Task ChangePrefix([Summary("New prefix for the bot's commands")]
                                    char prefix)
        {
            using (var ctx = new EmeraldBotContext())
            {
                try
                {
                    if (prefix == '@' || prefix == '#')
                        throw new Exception($"Impossible to set the prefix to {prefix}: too many possible conflicts.");

                    var server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id);
                    server.Prefix = $"{prefix}";
                    ctx.SaveChanges();
                    await ReplyAsync($"The prefix has been changed to {prefix}");
                }
                catch (Exception e)
                {
                    await ReplyAsync(e.Message);
                    Console.WriteLine($"Exception: {e.Message}:\n{e.StackTrace}");
                }
            }
        }

        [RequireOwner(Group = "Permission")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireGM(Group = "Permission")]
        [Command("setdicechannel")]
        [Summary("Sets the given channel as the dice channel")]
        public async Task SetDiceChannel([Summary("Channel to define as the dice channel")]
                                    IChannel chan)
        {
            using (var ctx = new EmeraldBotContext()) {
                try
                {
                    var server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id);
                    server.DiceChannelID = (long)chan.Id;
                    ctx.SaveChanges();
                    await ReplyAsync($"#{chan.Name} has been set as the dice channel.");

                }
                catch (Exception e)
                {
                    await ReplyAsync(e.Message);
                    Console.WriteLine($"Exception: {e.Message}:\n{e.StackTrace}");
                }
            }
        }

        [RequireOwner(Group = "Permission")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireGM(Group = "Permission")]
        [Command("setdicechannel")]
        [Summary("Sets the current channel as the dice channel")]
        public async Task SetDiceChannel()
        {
            using (var ctx = new EmeraldBotContext())
            {
                try
                {
                    var server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id);
                    server.DiceChannelID = (long)Context.Channel.Id;
                    ctx.SaveChanges();
                    await ReplyAsync($"#{Context.Channel.Name} has been set as the dice channel.");

                }
                catch (Exception e)
                {
                    await ReplyAsync(e.Message);
                    Console.WriteLine($"Exception: {e.Message}:\n{e.StackTrace}");
                }
            }
        }
    }
}
