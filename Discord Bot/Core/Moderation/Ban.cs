using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Bot.Core.Data;
using Discord_Bot.Modules.Channel_System;
using Discord_Bot.Modules.Role_System;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord_Bot.Core.Moderation
{
    public class BanCommands : ModuleBase<SocketCommandContext>
    { 
        [Command("ban")]
        public async Task Ban(IGuildUser user, [Remainder]string reason = "")
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User) ||
                (RoleManager.HasModRole((SocketGuildUser)Context.User) && (Context.Channel.Name == ":bellhop:-welcome-hall")))
            {
                string r = reason == "" ? "No reason specified" : reason;
                var acc = UserManager.GetAccount((SocketUser)user);
                acc.modData.bans.Add(new ModData.PenaltyData(Context.User.Id, Utilities.GetDate(), r));
                acc.modData.banned = true;
                UserManager.SaveAccounts();

                var channel = await user.GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync($"You have been banned from **{Context.Guild.Name}** for  `{r}`.");
                await Context.Guild.AddBanAsync(user, 0, r);
            }
        }

        [Command("voteban")]
        public async Task VoteBan(IGuildUser user, [Remainder]string reason = "")
        {
            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                string r = reason == "" ? "No reason specified" : reason;
                Voting.AddVote((SocketGuildUser)Context.User, "Ban", (SocketGuildUser)user, r);
            }
        }

        [Command("softban")]
        public async Task SoftBan(IGuildUser user, int days = 1, [Remainder]string reason = "")
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User) ||
                (RoleManager.HasModRole((SocketGuildUser)Context.User) && (Context.Channel.Name == ":bellhop:-welcome-hall")))
            {
                if (days > 7 || days < 0)
                {
                    var msg = await Context.Channel.SendMessageAsync("Softbanned days should be between `0` and `7`.");
                    await Task.Delay(5000);
                    await msg.DeleteAsync();
                    return;
                }

                string r = reason == "" ? "No reason specified" : reason;
                var acc = UserManager.GetAccount((SocketUser)user);
                acc.modData.softBans.Add(new ModData.PenaltyData(Context.User.Id, Utilities.GetDate(), r));
                UserManager.SaveAccounts();

                await (await user.GetOrCreateDMChannelAsync()).SendMessageAsync($"You have been soft banned from **{Context.Guild.Name}** for  `{r}`.");
                await Context.Guild.AddBanAsync(user, days, r);
            }
        }

        [Command("votesoftban")]
        public async Task VoteSoftBan(IGuildUser user, [Remainder]string reason = "")
        {
            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                string r = reason == "" ? "No reason specified" : reason;
                Voting.AddVote((SocketGuildUser)Context.User, "SoftBan", (SocketGuildUser)user, r);
            }
        }

        [Command("unban")]
        public async Task UnBan(ulong userId, [Remainder]string reason = "")
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                var bans = await Context.Guild.GetBansAsync();

                foreach (var ban in bans)
                {
                    if (ban.User.Id == userId)
                    {
                        foreach (var acc in UserManager.accounts)
                        {
                            if (acc.id == userId)
                            {
                                try
                                {
                                    string r = reason == "" ? "No reason specified" : reason;
                                    acc.modData.unBans.Add(new ModData.PenaltyData(Context.User.Id, Utilities.GetDate(), r));
                                }
                                catch
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | [Discord] : Could not write UnBan data for {ban.User.Username}.");
                                    Console.ResetColor();
                                }

                                acc.modData.banned = false;
                                UserManager.SaveAccounts();
                                break;
                            }
                        }

                        await Context.Guild.RemoveBanAsync(userId);
                        await Context.Channel.SendMessageAsync($"{userId} has been unbanned from the server.");

                        return;
                    }
                }

                await Context.Channel.SendMessageAsync($"{userId} has not been banned from the server.");
            }
        }

        [Command("voteunban")]
        public async Task VoteUnBan(IGuildUser user , [Remainder]string reason = "")
        {
            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                string r = reason == "" ? "No reason specified" : reason;
                Voting.AddVote((SocketGuildUser)Context.User, "UnBan", (SocketGuildUser)user, r);
            }
        }
    }
}
