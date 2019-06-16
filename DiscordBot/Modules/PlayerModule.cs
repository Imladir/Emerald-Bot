using Discord;
using Discord.Commands;
using Discord.WebSocket;
using EmeraldBot.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Modules
{
    [Group("player")]
    public class Players : ModuleBase<SocketCommandContext>
    {
        [Command("defaultcharacter")]
        [Alias("default")]
        [Summary("Sets the user's default character.")]
        public async Task SetDefaultCharacter([Remainder]
                                              [Summary("\nAlias of the user to become the new default character.\n" +
                                                       "It must be an existing character that the player owns.")]
                                              string nameOrAlias)
        {
            using (var ctx = new EmeraldBotContext())
            {
                try
                {
                    var player = ctx.Players.Single(x => x.DiscordID == (long)Context.User.Id);
                    player.LoadDefaultCharacters(ctx);
                    var character = ctx.GetPlayerCharacter(Context.Guild.Id, Context.User.Id, nameOrAlias);
                    var defChar = player.DefaultCharacters.SingleOrDefault(x => x.Server.DiscordID == (long)Context.Guild.Id);
                    if (defChar == null)
                        defChar = new Model.Servers.DefaultCharacter()
                        {
                            Player = player,
                            Character = character,
                            Server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id)
                        };
                    else
                        defChar.Character = character;
                    ctx.SaveChanges();
                    await ReplyAsync($"{character.Name} has been set as your default character");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Couldn't set '{nameOrAlias}' as default character: {e.Message}");
                    Console.WriteLine($"Couldn't set '{nameOrAlias}' as default character: {e.Message}\n{e.StackTrace}");
                }
            }
        }

        [Command("setPrivateChannel")]
        [Alias("private")]
        [Summary("Defines the current channel as your private one, where you'll be able to perform private operations, like displaying the full details of your characters.")]
        public async Task SetPrivateChannel()
        {
            using (var ctx = new EmeraldBotContext())
            {
                try
                {
                    var player = ctx.Players.Single(x => x.DiscordID == (long)Context.User.Id);
                    //player.LoadPrivateChannels(ctx);
                    var privateChannel = player.PrivateChannels.SingleOrDefault(x => x.Server.DiscordID == (long)Context.Guild.Id);
                    if (privateChannel == null)
                        privateChannel = new Model.Servers.PrivateChannel()
                        {
                            Player = player,
                            ChannelDiscordID = (long)Context.Channel.Id,
                            Server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id)
                        };
                    else
                        privateChannel.ChannelDiscordID = (long)Context.Channel.Id;
                    ctx.SaveChanges();
                    await ReplyAsync($"#{Context.Channel.Name} has been set as your private channel");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Couldn't set #{Context.Channel.Name} as your private channel: {e.Message}");
                    Console.WriteLine($"Couldn't set #{Context.Channel.Name} as your private channel: {e.Message}\n{e.StackTrace}");
                }
            }
        }

        [Command("setPrivateChannel")]
        [Alias("private")]
        [Summary("Defines the current channel as your private one, where you'll be able to perform private operations, like displaying the full details of your characters.")]
        public async Task SetPrivateChannel([Summary("\nThe channel to define as your private channel.")] ISocketMessageChannel chan)
        {
            using (var ctx = new EmeraldBotContext())
            {
                try
                {
                    var player = ctx.Players.Single(x => x.DiscordID == (long)Context.User.Id);
                    player.LoadPrivateChannels(ctx);
                    var privateChannel = player.PrivateChannels.SingleOrDefault(x => x.Server.DiscordID == (long)Context.Guild.Id);
                    if (privateChannel == null)
                        privateChannel = new Model.Servers.PrivateChannel()
                        {
                            Player = player,
                            ChannelDiscordID = (long)chan.Id,
                            Server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id)
                        };
                    else
                        privateChannel.ChannelDiscordID = (long)chan.Id;
                    ctx.SaveChanges();
                    await ReplyAsync($"#{chan.Name} has been set as your private channel");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Couldn't set #{chan.Name} as your private channel: {e.Message}");
                    Console.WriteLine($"Couldn't set #{chan.Name} as your private channel: {e.Message}\n{e.StackTrace}");
                }
            }
        }

        [Command("verbose")]
        [Alias("t")]
        [Summary("Changes the verbose state of the bot (on verbose, it will send you back every command it receives)")]
        public async Task SetVerbose()
        {
            using (var ctx = new EmeraldBotContext())
            {
                try
                {
                    var player = ctx.Players.Single(x => x.DiscordID == (long)Context.User.Id);
                    player.Verbose = !player.Verbose;
                    ctx.SaveChanges();

                    await ReplyAsync($"The bot verbose status (for you only) has been set to {(player.Verbose ? "Verbose" : "Silent")}");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Couldn't generate token: {e.Message}");
                    Console.WriteLine($"Couldn't generate token: {e.Message}\n{e.StackTrace}");
                }
            }
        }

        [Command("password")]
        [Alias("t")]
        [Summary("Resets the password to login on the website")]
        public async Task GenerateToken()
        {
            using (var ctx = new EmeraldBotContext())
            {
                try
                {
                    var player = ctx.Players.Single(x => x.DiscordID == (long)Context.User.Id);
                    var newPassword = player.GenerateToken();
                    ctx.SaveChanges();

                    await Context.User.SendMessageAsync($"Your new password is: {newPassword}");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Couldn't generate token: {e.Message}");
                    Console.WriteLine($"Couldn't generate token: {e.Message}\n{e.StackTrace}");
                }
            }
        }
    }
}
