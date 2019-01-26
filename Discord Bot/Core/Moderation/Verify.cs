using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Bot.Core.Data;
using Discord_Bot.Modules.Channel_System;
using Discord_Bot.Modules.Role_System;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace Discord_Bot.Core.Moderation
{
    public static class Verification
    {
        private static List<UnverifiedUser> timers = new List<UnverifiedUser>();

        private class UnverifiedUser : Timer
        {
            internal UnverifiedUser(SocketGuildUser _user)
            {
                user = _user;

                Interval = 1800000;
                AutoReset = false;
                Enabled = true;
            }

            internal SocketGuildUser user;
        }

        public static void Add(SocketGuildUser user)
        {
            user.AddRoleAsync(RoleManager.GetRole("⚠️ Unverified"));

            UnverifiedUser unverified = new UnverifiedUser(user);
            unverified.Elapsed += UnverifiedUser_ElapsedAsync;
        }

        public static async void Remove(SocketGuildUser user)
        {
            try
            {
                await user.RemoveRoleAsync(RoleManager.GetRole("⚠️ Unverified"));
                await user.AddRoleAsync(RoleManager.GetRole("Member"));
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | [Discord] : {user.Username} left the server.");
                Console.ResetColor();

                try
                {
                    var a = timers.Find(item => item.user == user);
                    timers.Remove(a);
                    return;
                }
                catch
                {
                    return;
                }
            }


            try
            {
                var a = timers.Find(item => item.user == user);
                timers.Remove(a);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | [Discord] : Error! failed to remove user {user.Username} from unverified list.");
                Console.ResetColor();

            }

        }

        private static async void UnverifiedUser_ElapsedAsync(object sender, ElapsedEventArgs e)
        {
            var unverified = (UnverifiedUser)sender;

            if (RoleManager.CheckUserRole(unverified.user, "⚠️ Unverified"))
            {
                if (!unverified.user.IsBot)
                {
                    try
                    {
                        var welcome = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "🛎-welcome-hall");
                        var invite = await welcome.CreateInviteAsync(86400, 1, true, true);

                        var dms = await unverified.user.GetOrCreateDMChannelAsync();
                        await dms.SendMessageAsync($"You have been kicked from {unverified.user.Guild.Name} for failing to verify. \n"
                            + $"You can attempt to join the server again through: {invite.Url} .");
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | [Discord] : Error! failed to obtain {unverified.user.Username}'s dms.");
                        Console.ResetColor();
                    }

                }

                try
                {
                    var acc = UserManager.GetAccount(unverified.user);
                    acc.modData.kicks.Add(new ModData.PenaltyData(Global.DiscordBotID, Utilities.GetDate(), "Failed to verify"));
                    UserManager.SaveAccounts();
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | [Discord] : Error! failed to obtain {unverified.user.Username}'s account.");
                    Console.ResetColor();
                }

                var user = Program._client.GetGuild(494596551394721828).GetUser(unverified.user.Id);

                if (user != null)
                {
                    var logs = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "📋-joins-and-leaves");

                    var embed = new EmbedBuilder();
                    var author = new EmbedAuthorBuilder();
                    var footer = new EmbedFooterBuilder();

                    author.WithName("Member Failed to Verify");
                    author.WithIconUrl(unverified.user.GetAvatarUrl());
                    embed.WithAuthor(author);

                    footer.WithText($"ID: {unverified.user.Id}");
                    embed.WithFooter(footer);

                    embed.WithCurrentTimestamp();
                    embed.WithColor(Color.Orange);
                    embed.WithThumbnailUrl(unverified.user.GetAvatarUrl());

                    var f0 = new EmbedFieldBuilder();
                    f0.WithIsInline(true);
                    f0.WithName("User");
                    f0.WithValue($"{unverified.user.Mention}");
                    embed.AddField(f0);

                    var f1 = new EmbedFieldBuilder();
                    f1.WithIsInline(true);
                    f1.WithName("Acc Created At");
                    f1.WithValue($"{unverified.user.CreatedAt.Month}/{unverified.user.CreatedAt.Day}/{unverified.user.CreatedAt.Year}");
                    embed.AddField(f1);

                    await logs.SendMessageAsync("", false, embed.Build());
                }

                Remove(unverified.user);
                await unverified.user.KickAsync();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | [Discord] : {unverified.user.Username} failed to verify.");
                Console.ResetColor();
            }
        }
    }

    public class VerifyCommands : ModuleBase<SocketCommandContext>
    {
        [Command("verify")]
        [Summary("Verifies mentioned user to grant them access to the server.")]
        public async Task Verify(IGuildUser user)
        {
            if (RoleManager.CheckUserRole((SocketGuildUser)Context.User, "Welcomer") || RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                if (RoleManager.CheckUserRole((SocketGuildUser)user, "⚠️ Unverified") || RoleManager.HasAdminRole((SocketGuildUser)Context.User))
                {
                    Verification.Remove((SocketGuildUser)user);

                    var channel = ChannelManager.GetTextChannel("🐾 Plebs", "🐺-general");
                    var role_picker = ChannelManager.GetTextChannel("🤖 Skynet", "role-picker");

                    await channel.SendMessageAsync($"Welcome {user.Mention}! {ChannelManager.GetEmote("pandaSip")} Use the {role_picker.Mention} channel to assign your roles.");
                }
            }
        }
    }
}
