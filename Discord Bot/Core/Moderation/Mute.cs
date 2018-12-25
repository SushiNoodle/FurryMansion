using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Bot.Core.Data;
using Discord_Bot.Modules.Channel_System;
using Discord_Bot.Modules.Role_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Discord_Bot.Core.Moderation
{
    public static class MuteData
    {
        private static Dictionary<SocketGuildUser, DateTime> timers = new Dictionary<SocketGuildUser, DateTime>();

        public static bool Find(SocketGuildUser user)
        {
            return timers.ContainsKey(user);
        }

        public static void Add(SocketGuildUser user)
        {
            timers.Add(user, DateTime.Now);
        }

        public static TimeSpan Remove(SocketGuildUser user)
        {
            var time = DateTime.Now - timers[user];
            timers.Remove(user);
            return time;
        }
    }

    public class MuteCommands : ModuleBase<SocketCommandContext>
    {
        [Command("mute")]
        [Summary("Gives mentioned user Prisoner role, removing access to the server except basement channel.")]
        public async Task Mute(IGuildUser user, [Remainder]string reason = "")
        {
            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                if (!MuteData.Find((SocketGuildUser)user))
                {
                    MuteData.Add((SocketGuildUser)user);
                    await user.AddRoleAsync(RoleManager.GetRole("Prisoner"));

                    string r = reason == "" ? "No reason specified" : reason;

                    var channel = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "⛓-basement-cell");
                    await channel.SendMessageAsync($"{user.Mention} has been muted for `{r}`.");

                    var acc = UserManager.GetAccount((SocketUser)user);
                    acc.modData.mutes.Add(new ModData.MuteData(Context.User.Id, Utilities.GetDate(), r));
                    UserManager.SaveAccounts();
                }
                else
                     await Context.Channel.SendMessageAsync($"{user.Mention} has already been muted.");

            }
        }

        [Command("unmute")]
        [Summary("Removes Prisoner role from mentioned user.")]
        public async Task UnMute(IGuildUser user)
        {
            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                if (MuteData.Find((SocketGuildUser)user))
                {
                    var timeSpan = MuteData.Remove((SocketGuildUser)user);
                    await user.RemoveRoleAsync(RoleManager.GetRole("Prisoner"));

                    var dm = await user.GetOrCreateDMChannelAsync();
                    await dm.SendMessageAsync($"You have been unmuted from **Furry Mansion**.");

                    var acc = UserManager.GetAccount((SocketUser)user);

                    var recentMute = acc.modData.mutes.LastOrDefault();
                    recentMute.time = $"{ timeSpan.Days}d { timeSpan.Hours}h { timeSpan.Minutes}m { timeSpan.Seconds}s";
                    UserManager.SaveAccounts();

                    var basement = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "⛓-basement-cell");
                    await basement.SendMessageAsync($"{user.Mention} was muted for {timeSpan.Days}d {timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s");
                }
                else
                    await Context.Channel.SendMessageAsync($"{user.Mention} is not muted.");
            }
        }
    }
}
