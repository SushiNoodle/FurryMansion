using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Discord_Bot.Core.Data;
using Discord_Bot.Modules.Channel_System;
using Discord_Bot.Modules.Role_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Modules.Voting_System
{
    public static class MemeVoting
    {
        public static List<ulong> Channels;
        public static List<ulong> MemeRoles;

        private static string MemeChannelsFile = "Resources/Channels/Memes.json";
        private static string MemeRolesFile = "Resources/Roles/MemeRoles.json";

        static MemeVoting()
        {
            if (DataStorage.SaveExists(MemeChannelsFile))
                Channels = DataStorage.LoadList<ulong>(MemeChannelsFile);
            else
            {
                Channels = new List<ulong>() { 495854179362013185, 495900317141958666 };
                SaveChannels();
            }

            if (DataStorage.SaveExists(MemeRolesFile))
                MemeRoles = DataStorage.LoadList<ulong>(MemeRolesFile);
            else
            {
                MemeRoles = new List<ulong>() { 538618666300866560 };
                SaveMemeRoles();
            }
        }

        private static void SaveChannels()
        {
            DataStorage.SaveList(Channels, MemeChannelsFile);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{ DateTime.Now.ToShortTimeString()} | [Discord] : Meme channels saved to {MemeChannelsFile}.");
            Console.ResetColor();
        }

        internal static bool RemoveChannel(ulong c)
        {
            bool r = Channels.Remove(c);

            if (r)
                SaveChannels();

            return r;
        }

        internal static void AddChannel(ulong c)
        {
            if (Channels.Contains(c))
                return;

            Channels.Add(c);
            SaveChannels();
        }

        private static void SaveMemeRoles()
        {
            DataStorage.SaveList(MemeRoles, MemeRolesFile);


            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{ DateTime.Now.ToShortTimeString()} | [Discord] : Meme roles saved to {MemeRolesFile}.");
            Console.ResetColor();
        }

        internal static bool RemoveMemeRole(ulong c)
        {
            bool r = MemeRoles.Remove(c);

            if (r)
                SaveMemeRoles();

            return r;
        }

        internal static void AddMemeRole(ulong c)
        {
            if (MemeRoles.Contains(c))
                return;

            MemeRoles.Add(c);
            SaveMemeRoles();
        }

        public static async void HandlePost(SocketUserMessage msg)
        {
            if (Channels.Contains(msg.Channel.Id))
            {
                if (msg.Attachments.Count == 0 && !(msg.Content.Contains("http://") || msg.Content.Contains("https://")))
                    return;

                await msg.AddReactionAsync(ChannelManager.GetEmote("yes"));
                await msg.AddReactionAsync(ChannelManager.GetEmote("no"));
            }
        }

        public static async void HandleVote(SocketReaction reaction)
        {
            if (Channels.Contains(reaction.Channel.Id))
            {
                if (reaction.User.Value.IsBot)
                    return;

                var rMsg = (RestUserMessage)await reaction.Channel.GetMessageAsync(reaction.MessageId);

                if (reaction.UserId == rMsg.Author.Id)
                {
                    await rMsg.RemoveReactionAsync(reaction.Emote, rMsg.Author);
                    await reaction.Channel.SendMessageAsync($"{rMsg.Author.Mention} stop reacting to your own memes you muffin >:C.");
                    return;
                }
                
                if (rMsg.Attachments.Count == 0)
                    return;

                var yes = ChannelManager.GetEmote("yes");
                var no = ChannelManager.GetEmote("no");

                int yes_count = 0;
                int no_count = 0;


                foreach (var u in await rMsg.GetReactionUsersAsync(yes))
                {
                    if (u.Id == reaction.UserId && reaction.Emote.Name != "yes")
                        await rMsg.RemoveReactionAsync(yes, u);


                    if (!u.IsBot)
                        ++yes_count;
                }

                foreach (var u in await rMsg.GetReactionUsersAsync(no))
                {
                    if (u.Id == reaction.UserId && reaction.Emote.Name != "no")
                        await rMsg.RemoveReactionAsync(no, u);

                    if (!u.IsBot)
                        ++no_count;
                }

                var guild = Global.Client.GetGuild(Global.GuildID);

                if (((yes_count + no_count) > (guild.MemberCount * 0.02)))
                {
                    var role = guild.GetRole(MemeRoles.FirstOrDefault());
                    var author = guild.GetUser(rMsg.Author.Id);

                    if (author == null)
                        return;

                    float percentage = (float)yes_count / (float)(yes_count + no_count);
                    if (percentage > 0.6)
                    {

                        if (!rMsg.IsPinned)
                        {
                            await rMsg.PinAsync();

                            if (author.Roles.Contains(role))
                                return;

                            int numpin = 0;
                            foreach(var c in Channels)
                            {
                                var _c = guild.GetTextChannel(c);
                                foreach( var p in await _c.GetPinnedMessagesAsync())
                                {
                                    if (p.Author.Id == rMsg.Author.Id)
                                        ++numpin;
                                }
                            }

                            if (numpin >= 10)
                            {
                                await author.AddRoleAsync(role);
                                await rMsg.Channel.SendMessageAsync($"{author.Mention} congrats! you now have no life, dab on the haters {ChannelManager.GetEmote("Dab")}.");
                            }
                        }
                    }
                    else
                    {
                        if (rMsg.IsPinned)
                            await rMsg.UnpinAsync();

                        if (!author.Roles.Contains(role))
                            return;

                        int numpin = 0;
                        foreach (var c in Channels)
                        {
                            var _c = guild.GetTextChannel(c);
                            foreach (var p in await _c.GetPinnedMessagesAsync())
                            {
                                if (p.Author.Id == rMsg.Author.Id)
                                    ++numpin;
                            }
                        }

                        if (numpin < 10)
                        {
                            await author.AddRoleAsync(role);
                            await rMsg.Channel.SendMessageAsync($"{author.Mention} dont go crying to alexa now {ChannelManager.GetEmote("Dab")}.");
                        }
                    }
                }
            }
        }
    }

    public class MemeCommands : ModuleBase<SocketCommandContext>
    {
        [Command("MemeChannel")]
        public async Task MemeChannel(string option, IGuildChannel c)
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                var guild = Global.Client.GetGuild(Global.GuildID);

                if (option.ToLowerInvariant() == "add")
                {
                    MemeVoting.AddChannel(c.Id);
                    await Context.Channel.SendMessageAsync($"{guild.GetTextChannel(c.Id).Mention} Added succesfully to the meme channels list.");
                }

                if (option.ToLowerInvariant() == "remove")
                {
                    if (MemeVoting.RemoveChannel(c.Id))
                        await Context.Channel.SendMessageAsync($"{guild.GetTextChannel(c.Id).Mention} Removed succesfully from the meme channels list.");
                    else
                        await Context.Channel.SendMessageAsync($"{guild.GetTextChannel(c.Id).Mention} Couldn't be found in the meme channels list.");
                }

                await Context.Message.DeleteAsync();
            }
        }

        [Command("MemeRole")]
        public async Task MemeRole(string option, [Remainder]string role = "")
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                var guild = Global.Client.GetGuild(Global.GuildID);
                var _role = RoleManager.GetRole(role);

                if (_role == null)
                {
                    await Context.Channel.SendMessageAsync($"`{role}` Does not exist.");
                    return;
                }

                if (option.ToLowerInvariant() == "add")
                {
                    MemeVoting.AddMemeRole(_role.Id);
                    await Context.Channel.SendMessageAsync($"{_role.Mention} Added succesfully to the meme roles list.");
                }

                if (option.ToLowerInvariant() == "remove")
                {
                    if (MemeVoting.RemoveMemeRole(_role.Id))
                        await Context.Channel.SendMessageAsync($"{_role.Mention} Removed succesfully from the meme roles list.");
                    else
                        await Context.Channel.SendMessageAsync($"{_role.Mention} Couldn't be found in the meme roles list.");
                }

                await Context.Message.DeleteAsync();
            }
        }
    }
}
