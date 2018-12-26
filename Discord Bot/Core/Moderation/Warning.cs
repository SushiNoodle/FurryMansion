using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Bot.Core.Data;
using Discord_Bot.Modules.Channel_System;
using Discord_Bot.Modules.Logging_System;
using Discord_Bot.Modules.Role_System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord_Bot.Core.Moderation
{
    public class WarningCommands : ModuleBase<SocketCommandContext>
    {
        [Command("warn")]
        public async Task Warn(IGuildUser user, [Remainder]string reason = "")
        {
            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                string r = reason == "" ? "No reason specified" : reason;
                var acc = UserManager.GetAccount((SocketUser)user);
                acc.modData.warnings.Add(new ModData.PenaltyData(Context.User.Id, Utilities.GetDate(), r));
                UserManager.SaveAccounts();

                LoggingManager.LogUserWarned((SocketGuildUser)user);

                if (acc.modData.warnings.Count >= 3)
                    Voting.AddVote((SocketGuildUser)Context.User, "Ban", (SocketGuildUser)user, "3 Warnings : " + reason);
                else
                    await Context.Channel.SendMessageAsync($"{user.Mention} has been warned for `{r}`. They now have `{acc.modData.warnings.Count}` warnings.");

                
            }
        }

        [Command("voteunwarn")]
        public async Task VoteUnwarn(IGuildUser user, [Remainder]string reason = "")
        {
            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                string r = reason == "" ? "No reason specified" : reason;

                var acc = UserManager.GetAccount((SocketUser)user);
                if (acc.modData.warnings.Count > 0)
                    Voting.AddVote((SocketGuildUser)Context.User, "UnWarn", (SocketGuildUser)user, r);
                else
                    await Context.Channel.SendMessageAsync($"{user.Mention} doesn't have any warnings to be removed.");
            }
        }

        [Command("unwarn")]
        public async Task UnWarn(IGuildUser user, [Remainder]string reason = "")
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                var acc = UserManager.GetAccount((SocketUser)user);
                if (acc.modData.warnings.Count > 0)
                {
                    acc.modData.warnings.RemoveAt(acc.modData.warnings.Count - 1);
                    UserManager.SaveAccounts();

                    string r = reason == "" ? "No reason specified" : reason;
                    LoggingManager.LogUserUnWarned((SocketGuildUser)user, (SocketGuildUser)Context.User, r);

                    await Context.Channel.SendMessageAsync($"{user.Mention} got a warning removed, they now have `{acc.modData.warnings.Count}` warnings.");
                }
                else
                    await Context.Channel.SendMessageAsync($"{user.Mention} doesn't have any warnings to be removed.");
            }
        }

        [Command("warnlist")]
        public async Task WarnList()
        {
            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                var accs = UserManager.accounts;

                SocketGuildUser user; int warns = 0, i = 0; string msg = "";
                foreach (var acc in accs)
                {
                    try { user = Context.Guild.GetUser(acc.id); }
                    catch { continue; }

                    if (user == null)
                        continue;

                    try { warns = acc.modData.warnings.Count; }
                    catch { continue; }

                    if (warns > 0)
                    {
                        msg += $"{user.Mention} : {warns} {(warns == 1 ? "warning" : "warnings")}.";
                        if (i++ < 10)
                            msg += "\n";
                        else
                        {
                            await Context.Channel.SendMessageAsync(msg);
                            i = 0; msg = "";
                        }
                        
                    } 
                }

                if (i < 10)
                    await Context.Channel.SendMessageAsync(msg);
            }
        }
    }
}
