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
    public static class NSFWVoting
    {
        public static List<ulong> Channels;
        public static List<ulong> Roles;

        private static string NSFWChannelsFile = "Resources/Channels/NSFWArt.json";
        private static string NSFWRolesfile = "Resources/Roles/NSFWRoles.json";

        static NSFWVoting()
        {
            if (DataStorage.SaveExists(NSFWChannelsFile))
                Channels = DataStorage.LoadList<ulong>(NSFWChannelsFile);
            else
            {
                Channels = new List<ulong>() {
                    495900419416129567,
                    496879679144329226,
                    495900442170097674,
                    495900457772908544,
                    495900477406314506,
                    532758233262063627,
                    506932738108948480,

                    495900501922021395,
                    506771720296988692,
                    506932670098440192,
                    504883062010806272,
                    504882753242791946};

                SaveChannels();
            }

            if (DataStorage.SaveExists(NSFWRolesfile))
                Roles = DataStorage.LoadList<ulong>(NSFWRolesfile);
            else
            {
                Roles = new List<ulong>() { 538732551372668928 };
                SaveRoles();
            }
        }

        private static void SaveChannels()
        {
            DataStorage.SaveList(Channels, NSFWChannelsFile);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{ DateTime.Now.ToShortTimeString()} | [Discord] : NSFW Art channels saved to {NSFWChannelsFile}.");
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

        private static void SaveRoles()
        {
            DataStorage.SaveList(Roles, NSFWRolesfile);


            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{ DateTime.Now.ToShortTimeString()} | [Discord] : NSFW roles saved to {NSFWRolesfile}.");
            Console.ResetColor();
        }

        internal static bool RemoveMemeRole(ulong c)
        {
            bool r = Roles.Remove(c);

            if (r)
                SaveRoles();

            return r;
        }

        internal static void AddRole(ulong c)
        {
            if (Roles.Contains(c))
                return;

            Roles.Add(c);
            SaveRoles();
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
                    float percentage = (float)yes_count / (float)(yes_count + no_count);
                    if (percentage > 0.6)
                    {
                        if (!rMsg.IsPinned)
                        {
                            await rMsg.PinAsync();

                            var role = guild.GetRole(Roles.FirstOrDefault());
                            var author = guild.GetUser(rMsg.Author.Id);

                            if (author.Roles.Contains(role))
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

                            if (numpin >= 10)
                            {
                                await author.AddRoleAsync(role);
                                await rMsg.Channel.SendMessageAsync($"{author.Mention} is a vewy knotty fur uwu.");
                            }
                        }
                    }
                    else
                    {
                        if (rMsg.IsPinned)
                            await rMsg.UnpinAsync();

                        var role = guild.GetRole(Roles.FirstOrDefault());
                        var author = guild.GetUser(rMsg.Author.Id);

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
                            await rMsg.Channel.SendMessageAsync($"{author.Mention} is now an innocent fur ;3.");
                        }
                    }
                }
            }
        }
    }

    public class NSFWArtCommands : ModuleBase<SocketCommandContext>
    {
        [Command("NSFWArtChannel")]
        public async Task NSFWArtChannel(string option, IGuildChannel c)
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                var guild = Global.Client.GetGuild(Global.GuildID);

                if (option.ToLowerInvariant() == "add")
                {
                    NSFWVoting.AddChannel(c.Id);
                    await Context.Channel.SendMessageAsync($"{guild.GetTextChannel(c.Id).Mention} Added succesfully to the NSFW channels list.");
                }

                if (option.ToLowerInvariant() == "remove")
                {
                    if (NSFWVoting.RemoveChannel(c.Id))
                        await Context.Channel.SendMessageAsync($"{guild.GetTextChannel(c.Id).Mention} Removed succesfully from the NSFW channels list.");
                    else
                        await Context.Channel.SendMessageAsync($"{guild.GetTextChannel(c.Id).Mention} Couldn't be found in the NSFW channels list.");
                }

                await Context.Message.DeleteAsync();
            }
        }

        [Command("NSFWRole")]
        public async Task NSFWRole(string option, [Remainder]string role = "")
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
                    NSFWVoting.AddRole(_role.Id);
                    await Context.Channel.SendMessageAsync($"{_role.Mention} Added succesfully to the NSFW roles list.");
                }

                if (option.ToLowerInvariant() == "remove")
                {
                    if (NSFWVoting.RemoveMemeRole(_role.Id))
                        await Context.Channel.SendMessageAsync($"{_role.Mention} Removed succesfully from the NSFW roles list.");
                    else
                        await Context.Channel.SendMessageAsync($"{_role.Mention} Couldn't be found in the NSFW roles list.");
                }

                await Context.Message.DeleteAsync();
            }
        }
    }
}
