using Discord;
using Discord.WebSocket;
using Discord_Bot.Modules.Channel_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace Discord_Bot
{
    internal static class Global
    {
        internal static DiscordSocketClient Client { get; set; }
        internal static ulong MessageIdToTrack { get; set; }

        internal static ulong StreamDiscordId = 0;

        internal struct Colors
        {
            internal static Color Green = new Color( 61, 182,  44);
            internal static Color Red   = new Color(255,   0,   0);
        }


        // To be removed.
        internal static ulong GuildID = 494596551394721828;
        internal static ulong DiscordBotID = 496214962314805250;
        internal static ulong TyID = 493738351472541697;
    }
}
