using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Bot.Core.Data;
using Discord_Bot.Modules.Channel_System;
using Discord_Bot.Modules.Logging_System;
using Discord_Bot.Modules.Role_System;
using Discord_Bot.Modules.Voting_System;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord_Bot.Core.Moderation
{
    public class KickCommands : ModuleBase<SocketCommandContext>
    {
        [Command("kick")]
        public async Task Kick(IGuildUser user, [Remainder]string reason = "")
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User) ||
                (RoleManager.HasModRole((SocketGuildUser)Context.User) && (Context.Channel.Name == ":bellhop:-welcome-hall")))
            {
                string r = reason == "" ? "No reason specified" : reason;
                var acc = UserManager.GetAccount((SocketUser)user);
                acc.modData.kicks.Add(new ModData.PenaltyData(Context.User.Id, Utilities.GetDate(), r));

                await (await user.GetOrCreateDMChannelAsync()).SendMessageAsync($"You have been kicked from **{Context.Guild.Name}** for  `{r}`.");

                UserManager.SaveAccounts();
                LoggingManager.LogUserKicked((SocketGuildUser)user);
                await user.KickAsync();


            }
        }

        [Command("votekick")]
        public async Task VoteKick(IGuildUser user, [Remainder]string reason = "")
        {
            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                string r = reason == "" ? "No reason specified" : reason;
                StaffVoting.AddVote((SocketGuildUser)Context.User, "Kick", (SocketGuildUser)user, r);
            }
        }

        [Command("userpurge")]
        public async Task UserPurge()
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                var office = Context.Guild.GetTextChannel(496875452166701066);
                var welcome = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "🛎-welcome-hall");
                int num = 0;

                string[] levels =
                {
                    "Cotton Picker (lvl 3)",
                    "Rice Picker (lvl 5)",
                    "Wall Builder (lvl 7)",
                    "Milk Bagger (lvl 11)",
                    "Diamond Picker (lvl 13)",
                    "Flat Earther (lvl 17)",
                    "Epic Gaymer (lvl 19)",
                    "Titty Streamer (lvl 23)",
                    "Ninja Sub (lvl 29)",
                    "Meme Reviewer (lvl 31)",
                    "9 year old (lvl 37)",
                    "No Lifer (lvl 41)"
                };

                foreach (var user in Context.Guild.Users)
                {
                    // Don't kick them if they're a bot or they're partnered.
                    if (user.IsBot || RoleManager.CheckUserRole(user, "💞 Partner"))
                        continue;

                    if ((DateTime.Today - user.JoinedAt).Value.Days > 30 && !RoleManager.CheckUserRole(user, levels))
                    {
                        await office.SendMessageAsync($"{user.Mention} just got kicked from the server due to innactivity. \n");
                        var invite = await welcome.CreateInviteAsync(86400, 1, true, true);

                        try
                        {
                            var dms = await user.GetOrCreateDMChannelAsync();
                            await dms.SendMessageAsync($"You've been kicked from {Context.Guild.Name} for inactivity! if you wish to join back again, you can, but remember to be active this time (at least get lvl 2) if you wish to stay. {invite.Url}");
                        }
                        catch
                        {
                            await office.SendMessageAsync($"{user.Mention} dms couldnt be reached.");
                        }

                        await user.KickAsync();
                        num++;
                    }
                }

                await office.SendMessageAsync($"{num} users got kicked for innactivity.");

            }
        }
    }
}
