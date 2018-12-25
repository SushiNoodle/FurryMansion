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
    public static class TempMuteData
    {
        private static List<muteTimer> timers = new List<muteTimer>();

        private class muteTimer : Timer
        {
            internal muteTimer(SocketGuildUser _user, uint time)
            {
                user = _user;

                Interval = time;
                AutoReset = false;
                Enabled = true;
            }

            internal SocketGuildUser user;
        }

        public static void Add(SocketGuildUser user, uint time, char type)
        {
            uint _time = 0;

            if (type == 's')
                _time = time * 1000;
            else if (type == 'm')
                _time = time * 60000;
            else if (type == 'h')
                _time = time * 3600000;

            muteTimer timer = new muteTimer(user, _time);
            timer.Elapsed += TempMute_ElapsedAsync;
        }

        private static async void TempMute_ElapsedAsync(object sender, ElapsedEventArgs e)
        {
            var timer = (muteTimer)sender;

            if (RoleManager.CheckUserRole(timer.user, "Prisoner"))
            {
                await timer.user.RemoveRoleAsync(RoleManager.GetRole("Prisoner"));

                var channel = await timer.user.GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync($"You have been unmuted from **Furry Mansion**.");

                MuteData.Remove(timer.user);
            }
        }
    }

    public class TempMuteCommand : ModuleBase<SocketCommandContext>
    {
        [Command("tempmute")]
        public async Task TempMute(IGuildUser user, uint time, char type, [Remainder]string reason = "")
        {
            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                if (!RoleManager.CheckUserRole((SocketGuildUser)user, "Prisoner"))
                {
                    TempMuteData.Add((SocketGuildUser)user, time, type);
                    MuteData.Add((SocketGuildUser)user);

                    await user.AddRoleAsync(RoleManager.GetRole("Prisoner"));
                    string r = reason == "" ? "No reason specified" : reason;

                    var channel = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "⛓-basement-cell");
                    await channel.SendMessageAsync($"{user.Mention} has been muted for `{r}`.");

                    var acc = UserManager.GetAccount((SocketUser)user);
                    acc.modData.mutes.Add(new ModData.MuteData(Context.User.Id, Utilities.GetDate(), r));
                    UserManager.SaveAccounts();
                }
            }
        }
    }
}
