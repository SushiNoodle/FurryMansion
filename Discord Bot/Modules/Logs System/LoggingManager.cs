using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Bot.Core.Data;
using Discord_Bot.Modules.Channel_System;
using System;

namespace Discord_Bot.Modules.Logging_System
{
    internal static class LoggingManager
    {
        public static async void LogUserJoined(SocketGuildUser user)
        {
            var logs = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "📋-joins-and-leaves");

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            var footer = new EmbedFooterBuilder();

            author.WithName("Member Joined");
            author.WithIconUrl(user.GetAvatarUrl());
            embed.WithAuthor(author);

            footer.WithText($"ID: {user.Id}");
            embed.WithFooter(footer);

            embed.WithCurrentTimestamp();
            embed.WithColor(Color.Green);
            embed.WithThumbnailUrl(user.GetAvatarUrl());

            var f0 = new EmbedFieldBuilder();
            f0.WithIsInline(true);
            f0.WithName("User");
            f0.WithValue($"{user.Mention}");
            embed.AddField(f0);

            var f1 = new EmbedFieldBuilder();
            f1.WithIsInline(true);
            f1.WithName("Acc Created At");
            f1.WithValue($"{user.CreatedAt.Day} {Utilities.GetMonth(user.CreatedAt.Month)} {user.CreatedAt.Year}");
            embed.AddField(f1);

            if (UserManager.AccountExists(user))
            {
                var acc = UserManager.GetAccount(user);

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
        public static async void LogUserLeft(SocketGuildUser user)
        {
            var logs = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "📋-joins-and-leaves");

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            var footer = new EmbedFooterBuilder();

            author.WithName("Member Left");
            author.WithIconUrl(user.GetAvatarUrl());
            embed.WithAuthor(author);

            footer.WithText($"ID: {user.Id}");
            embed.WithFooter(footer);

            embed.WithCurrentTimestamp();
            embed.WithColor(Color.Red);
            embed.WithThumbnailUrl(user.GetAvatarUrl());

            var f0 = new EmbedFieldBuilder();
            f0.WithIsInline(true);
            f0.WithName("User");
            f0.WithValue($"{user.Mention}");
            embed.AddField(f0);

            var f1 = new EmbedFieldBuilder();
            f1.WithIsInline(true);
            f1.WithName("Acc Created At");
            f1.WithValue($"{user.CreatedAt.Day} {Utilities.GetMonth(user.CreatedAt.Month)} {user.CreatedAt.Year}");
            embed.AddField(f1);

            await logs.SendMessageAsync("", false, embed.Build());

            Core.Moderation.Verification.Remove(user);

            var intro = ChannelManager.GetTextChannel("🐾 Plebs", "🌞-introductions");
            var intro_msgs = await intro.GetMessagesAsync().FlattenAsync();
            foreach (var msg in intro_msgs)
            {
                if (msg.Content.Contains(user.Mention))
                {
                    await msg.DeleteAsync();
                    break;
                }
            }

            var fursona = ChannelManager.GetTextChannel("🦄 Roleplay", "🐾-fursonas");
            var fursona_msgs = await fursona.GetMessagesAsync().FlattenAsync();
            foreach (var msg in fursona_msgs)
            {
                if (msg.Content.Contains(user.Mention))
                {
                    await msg.DeleteAsync();
                    break;
                }
            }
        }

        public static async void LogUserWarned(SocketGuildUser user)
        {

            var logs = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "📝-logging");
            var office = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "🚬-ty’s-office");
            var acc = UserManager.GetAccount(user);

            var warn = acc.modData.warnings[acc.modData.WarnCount() - 1];
            var staff = user.Guild.GetUser(warn.staff);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            var footer = new EmbedFooterBuilder();

            author.WithName("Member Warned");
            author.WithIconUrl(user.GetAvatarUrl());
            embed.WithAuthor(author);

            footer.WithText($"ID: {user.Id}");
            embed.WithFooter(footer);

            embed.WithCurrentTimestamp();
            embed.WithColor(Color.Red);
            embed.WithThumbnailUrl(user.GetAvatarUrl());

            var f0 = new EmbedFieldBuilder();
            f0.WithIsInline(true);
            f0.WithName("User");
            f0.WithValue($"{user.Mention}");
            embed.AddField(f0);

            var f1 = new EmbedFieldBuilder();
            f1.WithIsInline(true);
            f1.WithName("Warned By");
            f1.WithValue($"{staff.Mention}");
            embed.AddField(f1);

            var f2 = new EmbedFieldBuilder();
            f2.WithIsInline(false);
            f2.WithName("Reason");
            f2.WithValue(warn.reason);
            embed.AddField(f2);

            await office.SendMessageAsync("", false, embed.Build());
            await logs.SendMessageAsync("", false, embed.Build());
        }
        public static async void LogUserUnWarned(SocketGuildUser user, SocketGuildUser staff, string reason)
        {
            var logs = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "📝-logging");
            var office = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "🚬-ty’s-office");
            var acc = UserManager.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            var footer = new EmbedFooterBuilder();

            author.WithName("Member UnWarned");
            author.WithIconUrl(user.GetAvatarUrl());
            embed.WithAuthor(author);

            footer.WithText($"ID: {user.Id}");
            embed.WithFooter(footer);

            embed.WithCurrentTimestamp();
            embed.WithColor(Color.Orange);
            embed.WithThumbnailUrl(user.GetAvatarUrl());

            var f0 = new EmbedFieldBuilder();
            f0.WithIsInline(true);
            f0.WithName("User");
            f0.WithValue($"{user.Mention}");
            embed.AddField(f0);

            var f1 = new EmbedFieldBuilder();
            f1.WithIsInline(true);
            f1.WithName("UnWarned By");
            f1.WithValue($"{staff.Mention}");
            embed.AddField(f1);

            var f2 = new EmbedFieldBuilder();
            f2.WithIsInline(false);
            f2.WithName("Reason");
            f2.WithValue(reason);
            embed.AddField(f2);

            await office.SendMessageAsync("", false, embed.Build());
            await logs.SendMessageAsync("", false, embed.Build());
        }

        public static async void LogUserKicked(SocketGuildUser user)
        {
            var logs = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "📝-logging");
            var office = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "🚬-ty’s-office");
            var acc = UserManager.GetAccount(user);

            var kick = acc.modData.kicks[acc.modData.KickCount() - 1];
            var staff = user.Guild.GetUser(kick.staff);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            var footer = new EmbedFooterBuilder();

            author.WithName("Member Kicked");
            author.WithIconUrl(user.GetAvatarUrl());
            embed.WithAuthor(author);

            footer.WithText($"ID: {user.Id}");
            embed.WithFooter(footer);

            embed.WithCurrentTimestamp();
            embed.WithColor(Color.DarkRed);
            embed.WithThumbnailUrl(user.GetAvatarUrl());

            var f0 = new EmbedFieldBuilder();
            f0.WithIsInline(true);
            f0.WithName("User");
            f0.WithValue($"{user.Mention}");
            embed.AddField(f0);

            var f1 = new EmbedFieldBuilder();
            f1.WithIsInline(true);
            f1.WithName("Kicked By");
            f1.WithValue($"{staff.Mention}");
            embed.AddField(f1);

            var f2 = new EmbedFieldBuilder();
            f2.WithIsInline(false);
            f2.WithName("Reason");
            f2.WithValue(kick.reason);
            embed.AddField(f2);

            await office.SendMessageAsync("", false, embed.Build());
            await logs.SendMessageAsync("", false, embed.Build());
        }
        public static async void LogUserBanned(SocketUser user, SocketGuild guild)
        {
            var logs = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "📝-logging");
            var office = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "🚬-ty’s-office");
            var acc = UserManager.GetAccount(user);

            bool isBan = acc.modData.BanCount() != 0;
            var ban = isBan ? acc.modData.bans[acc.modData.BanCount() - 1] : acc.modData.softBans[acc.modData.SoftBanCount() - 1];
            var staff = guild.GetUser(ban.staff);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            var footer = new EmbedFooterBuilder();

            author.WithName("Member Banned");
            author.WithIconUrl(user.GetAvatarUrl());
            embed.WithAuthor(author);

            footer.WithText($"ID: {user.Id}");
            embed.WithFooter(footer);

            embed.WithCurrentTimestamp();
            embed.WithColor(Color.DarkRed);
            embed.WithThumbnailUrl(user.GetAvatarUrl());

            var f0 = new EmbedFieldBuilder();
            f0.WithIsInline(true);
            f0.WithName("User");
            f0.WithValue($"{user.Mention}");
            embed.AddField(f0);

            var f1 = new EmbedFieldBuilder();
            f1.WithIsInline(true);
            f1.WithName("Banned By");
            f1.WithValue($"{staff.Mention}");
            embed.AddField(f1);

            var f2 = new EmbedFieldBuilder();
            f2.WithIsInline(false);
            f2.WithName("Reason");
            f2.WithValue(ban.reason);
            embed.AddField(f2);

            await logs.SendMessageAsync("", false, embed.Build());
            await office.SendMessageAsync("", false, embed.Build());
        }
        public static async void LogUserUnBanned(SocketUser user, SocketGuild guild)
        {
            var logs = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "📝-logging");
            var office = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "🚬-ty’s-office");
            var acc = UserManager.GetAccount(user);


            ModData.PenaltyData unBan = new ModData.PenaltyData();
            SocketGuildUser staff = null;

            try
            {
                unBan = acc.modData.unBans[acc.modData.UnBanCount() - 1]; ;
                staff = guild.GetUser(unBan.staff);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | [Discord] : Could not retrieve UnBan data for {user.Username}.");
                Console.ResetColor();
            }

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            var footer = new EmbedFooterBuilder();

            author.WithName("Member UnBanned");
            author.WithIconUrl(user.GetAvatarUrl());
            embed.WithAuthor(author);

            footer.WithText($"ID: {user.Id}");
            embed.WithFooter(footer);

            embed.WithCurrentTimestamp();
            embed.WithColor(Color.Green);
            embed.WithThumbnailUrl(user.GetAvatarUrl());

            var f0 = new EmbedFieldBuilder();
            f0.WithIsInline(true);
            f0.WithName("User");
            f0.WithValue($"{user.Mention}");
            embed.AddField(f0);

            var f1 = new EmbedFieldBuilder();
            f1.WithIsInline(true);
            f1.WithName("UnBanned By");
            f1.WithValue($"{(staff == null ? "Staff not found" : staff.Mention)}");
            embed.AddField(f1);

            var f2 = new EmbedFieldBuilder();
            f2.WithIsInline(false);
            f2.WithName("Reason");
            f2.WithValue((unBan.reason == null ? "No reason specified" : unBan.reason));
            embed.AddField(f2);

            await logs.SendMessageAsync("", false, embed.Build());
            await office.SendMessageAsync("", false, embed.Build());
        }
    }
}
