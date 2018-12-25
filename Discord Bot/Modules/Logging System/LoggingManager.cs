using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Bot.Core.Data;
using Discord_Bot.Modules.Channel_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Modules.Logging_System
{
    internal static class LoggingManager
    {
        public static async void OnJoin(SocketGuildUser arg)
        {
            var logs = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "📋-joins-and-leaves");

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            var footer = new EmbedFooterBuilder();

            author.WithName("Member Joined");
            author.WithIconUrl(arg.GetAvatarUrl());
            embed.WithAuthor(author);

            footer.WithText($"ID: {arg.Id}");
            embed.WithFooter(footer);

            embed.WithCurrentTimestamp();
            embed.WithColor(Color.Green);
            embed.WithThumbnailUrl(arg.GetAvatarUrl());

            var f0 = new EmbedFieldBuilder();
            f0.WithIsInline(true);
            f0.WithName("User");
            f0.WithValue($"{arg.Mention}");
            embed.AddField(f0);

            var f1 = new EmbedFieldBuilder();
            f1.WithIsInline(true);
            f1.WithName("Acc Created At");
            f1.WithValue($"{arg.CreatedAt.Day} {Utilities.GetMonth(arg.CreatedAt.Month)} {arg.CreatedAt.Year}");
            embed.AddField(f1);

            if (UserManager.AccountExists(arg))
            {
                var acc = UserManager.GetAccount(arg);

                var f2 = new EmbedFieldBuilder();
                f2.WithIsInline(true);
                f2.WithName("Warnings");
                f2.WithValue(acc.modData.WarnCount().ToString());
                embed.AddField(f2);

                var f3 = new EmbedFieldBuilder();
                f3.WithIsInline(true);
                f3.WithName("Mutes");
                f3.WithValue(acc.modData.MuteCount().ToString());
                embed.AddField(f3);

                var f4 = new EmbedFieldBuilder();
                f4.WithIsInline(true);
                f4.WithName("Kicks");
                f4.WithValue(acc.modData.KickCount().ToString());
                embed.AddField(f4);

                var f5 = new EmbedFieldBuilder();
                f5.WithIsInline(true);
                f5.WithName("SoftBans");
                f5.WithValue(acc.modData.SoftBanCount().ToString());
                embed.AddField(f5);

                var f6 = new EmbedFieldBuilder();
                f6.WithIsInline(true);
                f6.WithName("Bans");
                f6.WithValue(acc.modData.BanCount().ToString());
                embed.AddField(f6);
            }

            await logs.SendMessageAsync("", false, embed.Build());
        }
    }
}
