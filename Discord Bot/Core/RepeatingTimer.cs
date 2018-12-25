using Discord;
using Discord.WebSocket;
using Discord_Bot.Modules.Channel_System;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace Discord_Bot.Core
{
    static internal class RepeatingTimer
    {
        private static Dictionary<string, Timer> timers;

        internal static Task StartTimer()
        {
            timers = new Dictionary<string, Timer>
            {
                {"discordTimer", new Timer { Interval = 300000, AutoReset = true, Enabled = true } }
            };

            timers["discordTimer"].Elapsed += OnDiscordTimerTick;

            return Task.CompletedTask;
        }

        private static void OnDiscordTimerTick(object sender, ElapsedEventArgs e)
        {
            // await channel.SendMessageAsync("Timer Ticked..");
        }
    }
}
