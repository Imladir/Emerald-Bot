using Discord;
using Discord.Commands;
using EmeraldBot.Bot.Tools;
using EmeraldBot.Model;
using EmeraldBot.Model.Characters;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmeraldBot.Bot.Modules
{
    public class AutoFormat : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Summary("Formats a message so that it can be displayed as in-character")]
        public async Task Say([Summary("The text to format, with an optional parameter or $alias")] [Remainder] CommandOptions<PC> options)
        {
            try
            {
                await Talk(options.Target, options.Text);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Couldn't send message: {e.Message}");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        [Command("npc")]
        [Summary("Creates a simple block of text for an NPC without creating an actual sheet. **Only the GM can use it.**")]
        public async Task NPCSay([Remainder]
                                  [Summary("By default, only needs the text to display. It will appear as if *A Passerby* had just talked.\n" +
                                           "If you want, you can give more details by supplying **key=value** pairs:\n" +
                                           "- **clan=xxx**: defines the color of the border and the default image displayed (clan mon). The name becomes *A xxx Clan member*.\n" +
                                           "- **name=xxx**: changes the name to xxx\n" +
                                           "- **icon=\"xxx.jpg\"**: must be a valid URL of an image (the actual image, not a page containing the image) and will display it instead of the clan mon.")]
                                  CommandOptions<Character> options)
        {
            using (var ctx = new EmeraldBotContext())
            {
                try
                {
                    var target = new PC();
                    target.Update(ctx, options.Params);

                    if (target.Clan != null && target.Icon == "") target.Icon = target.Clan.Icon;
                    await Talk(target, options.Text);
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Couldn't send message: {e.Message}");
                    Console.WriteLine($"{e.Message}\n{e.StackTrace}");
                }
            }
        }

        private async Task Talk(PC target, string text)
        {
            using (var ctx = new EmeraldBotContext())
            {
                var emd = AutoFormater.Format(target, text);
                var res = await Context.Channel.SendMessageAsync("", false, emd.Build());

                var player = ctx.Users.Single(x => x.DiscordID == (long)Context.User.Id);
                var server = ctx.Servers.Single(x => x.DiscordID == (long)Context.Guild.Id);
                ctx.Entry(player).Collection(x => x.Messages).Load();

                var message = new Model.Servers.Message()
                {
                    Data = JsonConvert.SerializeObject(emd),
                    DiscordChannelID = (long)Context.Channel.Id,
                    DiscordMessageID = (long)res.Id,
                    LastUpdated = DateTime.UtcNow,
                    Server = server,
                    Player = player,
                };
                if (target.ID > 0) message.Character = target;

                //player.Messages.Add(message);
                ctx.Messages.Add(message);
                ctx.SaveChanges();

                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
        }

        [Command("editlastmessage")]
        [Alias("edit")]
        [Summary("Edits the last message the bot formated for you.")]
        public async Task EditLastMessage([Remainder]
                                          [Summary("The new text replacing the old one.")]
                                          string data)
        {
            using (var ctx = new EmeraldBotContext())
            {
                try
                {
                    var message = ctx.Messages.Where(x => x.DiscordChannelID == (long)Context.Channel.Id
                                                       && x.Server.DiscordID == (long)Context.Guild.Id
                                                       && x.Player.DiscordID == (long)Context.User.Id)
                                              .OrderByDescending(x => x.LastUpdated).FirstOrDefault();

                    Console.WriteLine($"I found messages? {message != null}");
                    if (message == null) return;

                    var emd = JsonConvert.DeserializeObject<EmbedBuilder>(message.Data);

                    // Getting color again because it's fucked up
                    Regex reColor = new Regex("Color.{4}RawValue.{2}(?<color>\\d+)");
                    Match m = reColor.Match(message.Data);
                    emd.WithColor(new Color(uint.Parse(m.Groups["color"].Value)));

                    emd.WithDescription(AutoFormater.SimpleFormat(data));

                    IUserMessage msg = (IUserMessage)await Context.Channel.GetMessageAsync((ulong)message.DiscordMessageID);
                    await msg.ModifyAsync(x => x.Embed = emd.Build());
                    message.Data = JsonConvert.SerializeObject(emd);
                    message.LastUpdated = DateTime.UtcNow;
                    ctx.SaveChanges();
                    await Context.Channel.DeleteMessageAsync(Context.Message);
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Couldn't edit message: {e.Message}");
                    Console.WriteLine($"{e.Message}\n{e.StackTrace}");
                }
            }
        }

        [Command("deletelastmessage")]
        [Summary("Deletes the last message the bot formated for you. If that message has already been deleted, this command will do nothing.")]
        public async Task DeleteLastMessage()
        {
            using (var ctx = new EmeraldBotContext())
            {
                var message = ctx.Messages.Where(x => x.DiscordChannelID == (long)Context.Channel.Id
                                                   && x.Server.DiscordID == (long)Context.Guild.Id
                                                   && x.Player.DiscordID == (long)Context.User.Id)
                                          .OrderByDescending(x => x.LastUpdated).FirstOrDefault();
                if (message == null) return;

                IUserMessage msg = (IUserMessage)await Context.Channel.GetMessageAsync((ulong)message.DiscordMessageID);
                await msg.DeleteAsync();
                ctx.Messages.Remove(message);
                ctx.SaveChanges();
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
        }
    }
}
