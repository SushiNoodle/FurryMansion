using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Discord_Bot.Core.Data;
using Discord_Bot.Modules.Channel_System;
using Discord_Bot.Modules.Logging_System;
using Discord_Bot.Modules.Role_System;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord_Bot.Modules.Voting_System
{
    public enum request
    {
        Ban,
        SoftBan,
        Kick,
        UnBan,
        UnWarn
    }

    public static class StaffVoting
    {
        public static List<IUserMessage> StaffVotes = new List<IUserMessage>();
        public static List<IUserMessage> PublicVotes = new List<IUserMessage>();


        public static async void AddVote(SocketGuildUser staff, string request, SocketGuildUser user, string reason)
        {
            var office = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "🚬-ty’s-office");

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            var _staff = new EmbedFieldBuilder();
            var _request = new EmbedFieldBuilder();
            var _user = new EmbedFieldBuilder();
            var _reason = new EmbedFieldBuilder();

            author.WithName(user.Username);
            author.WithIconUrl(user.GetAvatarUrl());

            _staff.WithName("Staff");
            _staff.WithValue(staff.Mention);
            _staff.WithIsInline(true);

            _request.WithName("Request");
            _request.WithValue(request);
            _request.WithIsInline(true);

            _user.WithName("User");
            _user.WithValue(user.Mention);
            _user.WithIsInline(true);

            _reason.WithName("Reason");
            _reason.WithValue(reason);
            _reason.WithIsInline(false);

            var mod = RoleManager.GetRole("🤖 Bully Hunter");

            embed.WithCurrentTimestamp();
            embed.WithColor(Color.Red);
            embed.WithAuthor(author);
            embed.WithDescription($"3 {mod.Mention} votes in favor are needed to perform this action.");
            embed.AddField(_staff);
            embed.AddField(_request);
            embed.AddField(_user);
            embed.AddField(_reason);

            var msg = await office.SendMessageAsync("", false, embed.Build());
            await msg.AddReactionAsync(ChannelManager.GetEmote("yes"));
            await msg.AddReactionAsync(ChannelManager.GetEmote("no"));

            StaffVotes.Add(msg);
        }

        public static async void HandleStaffVote(SocketReaction reaction)
        {
            var office = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "🚬-ty’s-office");

            if (reaction.Channel.Id != office.Id)
                return;

            foreach (var vote in StaffVotes)
            {
                if (vote.Id == reaction.MessageId)
                {
                    if (!RoleManager.HasModRole((SocketGuildUser)reaction.User))
                    {
                        var rMessage = (RestUserMessage)(await office.GetMessageAsync(reaction.MessageId));
                        await rMessage.RemoveReactionAsync(reaction.Emote, (SocketUser)reaction.User);
                        break;
                    }

                    var yes = ChannelManager.GetEmote("yes");
                    var no = ChannelManager.GetEmote("no");

                    var y = (await vote.GetReactionUsersAsync(ChannelManager.GetEmote("yes"))).Count;
                    var n = (await vote.GetReactionUsersAsync(ChannelManager.GetEmote("no"))).Count;

                    if (y == 0 && n == 0)
                        break;

                    if ((y - n) > 2)
                    {

                        var result = from f in vote.Embeds.FirstOrDefault().Fields
                                     where f.Name == "Request"
                                     select f;

                        var type = result.FirstOrDefault().Value;


                        result = from f in vote.Embeds.FirstOrDefault().Fields
                                 where f.Name == "Reason"
                                 select f;

                        var reason = result.FirstOrDefault().Value;

                        result = from f in vote.Embeds.FirstOrDefault().Fields
                                 where f.Name == "Staff"
                                 select f;

                        var staff_mention = result.FirstOrDefault().Value;

                        result = from f in vote.Embeds.FirstOrDefault().Fields
                                 where f.Name == "User"
                                 select f;

                        var user_mention = result.FirstOrDefault().Value;

                        string _staffID = "";
                        foreach (var c in staff_mention)
                        {
                            if (c != '<' && c != '@' && c != '>' && c != '!')
                                _staffID += c;
                        }
                        ulong staffID = ulong.Parse(_staffID);

                        string _userID = "";
                        foreach (var c in user_mention)
                        {
                            if (c != '<' && c != '@' && c != '>' && c != '!')
                                _userID += c;
                        }
                        ulong userID = ulong.Parse(_userID);

                        var staff = Global.Client.GetUser(staffID);
                        var user = Global.Client.GetUser(userID);
                        var acc = UserManager.GetAccount(user);

                        var guild = Global.Client.GetGuild(Global.GuildID);

                        string r = reason == "" ? "No reason specified" : reason;

                        switch (Enum.Parse(typeof(request), type))
                        {
                            case request.Ban:
                                {
                                    acc.modData.bans.Add(new ModData.PenaltyData(staff.Id, Utilities.GetDate(), r));
                                    acc.modData.banned = true;
                                    UserManager.SaveAccounts();

                                    var channel = await user.GetOrCreateDMChannelAsync();
                                    await channel.SendMessageAsync($"You have been banned from **{guild.Name}** for  `{r}`.");

                                    await guild.AddBanAsync(user, 0, r);
                                    break;
                                }

                            case request.SoftBan:
                                {
                                    acc.modData.softBans.Add(new ModData.PenaltyData(staff.Id, Utilities.GetDate(), r));
                                    UserManager.SaveAccounts();

                                    var channel = await user.GetOrCreateDMChannelAsync();
                                    await channel.SendMessageAsync($"You have been softbanned from **{guild.Name}** for  `{r}`.");
                                    await guild.AddBanAsync(user, 1, r);
                                    await guild.RemoveBanAsync(user);
                                    break;
                                }

                            case request.Kick:
                                {
                                    acc.modData.kicks.Add(new ModData.PenaltyData(staff.Id, Utilities.GetDate(), r));
                                    UserManager.SaveAccounts();

                                    var channel = await user.GetOrCreateDMChannelAsync();
                                    await channel.SendMessageAsync($"You have been kicked from **{guild.Name}** for  `{r}`.");

                                    LoggingManager.LogUserKicked((SocketGuildUser)user);
                                    await guild.GetUser(user.Id).KickAsync();
                                    break;
                                }

                            case request.UnBan:
                                {
                                    var bans = await guild.GetBansAsync();

                                    var bansearch = from b in bans
                                                    where b.User.Id == user.Id
                                                    select b;

                                    var ban = bansearch.FirstOrDefault();

                                    if (ban == null)
                                    {
                                        await office.SendMessageAsync($"{user.Mention} is not banned from this server.");
                                        break;
                                    }

                                    acc.modData.unBans.Add(new ModData.PenaltyData(user.Id, Utilities.GetDate(), reason));
                                    acc.modData.banned = false;
                                    UserManager.SaveAccounts();

                                    try { await (await user.GetOrCreateDMChannelAsync()).SendMessageAsync($"You have been unbanned from **{guild.Name}**."); }
                                    catch { }

                                    await guild.RemoveBanAsync(user);

                                    break;
                                }

                            case request.UnWarn:
                                {
                                    if (acc.modData.warnings.Count > 0)
                                    {
                                        acc.modData.warnings.RemoveAt(acc.modData.warnings.Count - 1);
                                        UserManager.SaveAccounts();

                                        LoggingManager.LogUserUnWarned((SocketGuildUser)user, (SocketGuildUser)staff, r);

                                        await office.SendMessageAsync($"{user.Mention} got a warning removed, they now have `{acc.modData.warnings.Count}` warnings.");
                                    }
                                    else
                                        await office.SendMessageAsync($"{user.Mention} doesn't have any warnings to be removed.");

                                    break;
                                }
                        }

                        StaffVotes.Remove(vote);

                        try { await vote.DeleteAsync(); }
                        catch { return; }
                    }

                    else if ((y - n) < -2)
                    {
                        StaffVotes.Remove(vote);
                        await vote.DeleteAsync();
                        await office.SendMessageAsync("Voting got cancelled.");
                    }

                    break;
                }
            }
        }
        }
}
