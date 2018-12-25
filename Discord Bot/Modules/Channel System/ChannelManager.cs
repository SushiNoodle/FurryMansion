using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;

namespace Discord_Bot.Modules.Channel_System
{


    internal static class ChannelManager
    {
        private static Dictionary<Tuple<string, string>, System.UInt64> TextChannels;
        private static Dictionary<Tuple<string, string>, System.UInt64> VoiceChannels;
        private static Dictionary<string, UInt64> Emotes;


        static ChannelManager()
        {
            Initialize();
        }


        internal static void Initialize()
        {
            SocketGuild guild = Global.Client.GetGuild(Global.GuildID);

            // Allocate memory for both channel dictionaries.
            TextChannels = new Dictionary<Tuple<string, string>, UInt64>();
            VoiceChannels = new Dictionary<Tuple<string, string>, UInt64>();
            Emotes = new Dictionary<string, UInt64>();

            // Save all text cchannels.
            foreach (SocketTextChannel i in guild.TextChannels)
            {
                TextChannels[Tuple.Create(i.Category.Name, i.Name)] = i.Id;
            }

            // Save all voice channels.
            foreach (SocketVoiceChannel i in guild.VoiceChannels)
            {
                VoiceChannels[Tuple.Create(i.Category.Name, i.Name)] = i.Id;
            }

            foreach (GuildEmote i in guild.Emotes)
            {
                Emotes[i.Name] = i.Id;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | [Discord] : Channel manager initialized.");
            Console.ResetColor();
        }

        internal static UInt64 GetChannelId(string category, string channel)
        {
            return TextChannels[Tuple.Create(category, channel)];
        }

        internal static SocketTextChannel GetTextChannel(string category, string channel)
        {
            UInt64 id = GetChannelId(category, channel);
            return Global.Client.GetGuild(Global.GuildID).GetTextChannel(id);
        }

        internal static SocketVoiceChannel GetVoiceChannel(string category, string channel)
        {
            UInt64 id = GetChannelId(category, channel);
            return Global.Client.GetGuild(Global.GuildID).GetVoiceChannel(id);
        }

        internal static Emote GetEmote(string name)
        {
            var v = Emote.Parse($"<:{name}:{Emotes[name]}>");
            return v;
        }
    }
}
