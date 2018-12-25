using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Bot.Modules.Channel_System;
using Discord_Bot.Modules.Role_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Core.Data
{
    public class UserCommands : ModuleBase<SocketCommandContext>
    {
        internal async Task<EmbedBuilder> GetProfileInfo(SocketGuildUser user)
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            UserAccount acc = UserManager.GetAccount(user);

            author.WithName(user.Username);
            author.WithIconUrl(user.GetAvatarUrl());
            embed.WithAuthor(author);

            embed.WithThumbnailUrl(user.GetAvatarUrl());
            embed.WithColor(Color.Blue);

            var f1 = new EmbedFieldBuilder();
            f1.WithIsInline(true);
            f1.WithName("Joined At");
            f1.WithValue($"```{user.JoinedAt.Value.Day} {Utilities.GetMonth(user.JoinedAt.Value.Month)} {user.JoinedAt.Value.Year}```");
            embed.AddField(f1);

            UserProfile profile = acc.GetUserProfile();
            var sonas = acc.GetFursonas();

            if (profile.age != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Age");
                f.WithValue("```" + profile.age + " years old```");
                embed.AddField(f);
            } 
            if (profile.location != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Location");
                f.WithValue("```" + profile.location + "```");
                embed.AddField(f);
            }

            if (sonas.Count > 0)
            {
                string sonalist = "";
                foreach (var s in sonas)
                    sonalist += s.Value.name + "\n";

                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Fursonas");
                f.WithValue("```" + sonalist + "```");
                embed.AddField(f);
            }

            if (profile.telegram != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Telegram");
                f.WithValue("```" + $"@{profile.telegram}" + "```");
                embed.AddField(f);
            }

            if (profile.furaffinity != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Furaffinity");
                f.WithValue($"https://furaffinity.net/user/{profile.furaffinity}");
                embed.AddField(f);
            }

            if (profile.battlenet != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("BattleNet ID");
                f.WithValue("```" + $"{profile.battlenet}" + "```");
                embed.AddField(f);
            }

            if (profile.steam != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Steam ID");
                f.WithValue($"http://steamcommunity.com/id/{profile.steam}");
                embed.AddField(f);
            }

            if (profile.psn != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("PlayStation Network");
                f.WithValue("```" + $"{profile.psn}" + "```");
                embed.AddField(f);
            }

            if (profile.xbox != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("XBox GamerTag");
                f.WithValue("```" + $"{profile.xbox}" + "```");
                embed.AddField(f);
            }

            if (profile.hobbies != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(false);
                f.WithName("Hobbies");
                f.WithValue("```" + $"{profile.hobbies}" + "```");
                embed.AddField(f);
            }

            if (profile.aboutme != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(false);
                f.WithName("About Me");
                f.WithValue("```" + $"{profile.aboutme}" + "```");
                embed.AddField(f);
            }

            return embed;
        }

        // internal async Task<EmbedBuilder> GetFursonaInfo(SocketGuildUser user, string sona) { }

        [Command("postprofile")]
        public async Task PostProfile(IGuildUser user)
        {
            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                var channel = ChannelManager.GetTextChannel("🐾 Plebs", "🌞-introductions");

                var messages = await channel.GetMessagesAsync().FlattenAsync();
                foreach (var msg in messages)
                {
                    if (msg.Content.Contains(user.Mention))
                    {
                        await msg.DeleteAsync();
                        break;
                    } 
                }

                await channel.SendMessageAsync(user.Mention, false, (await GetProfileInfo((SocketGuildUser)user)).Build());
            }
        }

        [Command("age"), Summary("Set your age on your profile (e.g. 9).")]
        public async Task Age([Remainder]string age = "")
        {
            UserAccount acc = UserManager.GetAccount(Context.User);
            var profile = acc.GetUserProfile();
            profile.age = age;
            UserManager.SaveAccounts();

            var msg = await Context.Channel.SendMessageAsync($"Age set to `{(age == "" ? "None" : age)}` succesfully.");
            await Task.Delay(2500);
            await msg.DeleteAsync();
            await Context.Message.DeleteAsync();
        }

        [Command("view"), Summary("View a user's profile (including your own)")]
        public async Task View(IGuildUser user)
        {
            await Context.Channel.SendMessageAsync("", false, (await GetProfileInfo((SocketGuildUser)user)).Build());
        }

        [Command("location"), Summary("Set your location on your profile (e.g. Australia, Florida, or Paris).")]
        public async Task Location([Remainder]string location = "")
        {
            UserAccount acc = UserManager.GetAccount(Context.User);
            var profile = acc.GetUserProfile();
            profile.location = location;
            UserManager.SaveAccounts();

            var msg = await Context.Channel.SendMessageAsync($"Location set to `{(location == "" ? "None" : location)}` succesfully.");
            await Task.Delay(2500);
            await msg.DeleteAsync();
            await Context.Message.DeleteAsync();
        }

        [Command("telegram"), Summary("Set your Telegram ID.")]
        public async Task Telegram([Remainder]string telegram = "")
        {
            UserAccount acc = UserManager.GetAccount(Context.User);
            var profile = acc.GetUserProfile();
            profile.telegram = telegram;
            UserManager.SaveAccounts();

            var msg = await Context.Channel.SendMessageAsync($"Telegram set to `@{(telegram == "" ? "None" : telegram)}` succesfully.");
            await Task.Delay(2500);
            await msg.DeleteAsync();
            await Context.Message.DeleteAsync();
        }

        [Command("furaffinity"), Summary("Set your Furaffinity Link.")]
        public async Task FurAffinity([Remainder]string FA = "")
        {
            UserAccount acc = UserManager.GetAccount(Context.User);
            var profile = acc.GetUserProfile();
            profile.furaffinity = FA;
            UserManager.SaveAccounts();

            var msg = await Context.Channel.SendMessageAsync($"Telegram set to {(FA == "" ? "`None`" : "https://furaffinity.net/user/" + profile.furaffinity)} succesfully.");
            await Task.Delay(2500);
            await msg.DeleteAsync();
            await Context.Message.DeleteAsync();
        }

        [Command("battlenet"), Summary("Set your BattleNet ID.")]
        public async Task BattleNet([Remainder]string battlenet = "")
        {
            UserAccount acc = UserManager.GetAccount(Context.User);
            var profile = acc.GetUserProfile();
            profile.battlenet = battlenet;
            UserManager.SaveAccounts();

            var msg = await Context.Channel.SendMessageAsync($"BattleNet ID set to `{(battlenet == "" ? "None" : battlenet)}` succesfully.");
            await Task.Delay(2500);
            await msg.DeleteAsync();
            await Context.Message.DeleteAsync();
        }

        [Command("steam"), Summary("Set your Steam Link.")]
        public async Task Steam([Remainder]string steam = "")
        {
            UserAccount acc = UserManager.GetAccount(Context.User);
            var profile = acc.GetUserProfile();
            profile.steam = steam;
            UserManager.SaveAccounts();

            var msg = await Context.Channel.SendMessageAsync($"Steam profile set to {(steam == "" ? "`None`" : "http://steamcommunity.com/id/" + profile.steam)} succesfully.");
            await Task.Delay(2500);
            await msg.DeleteAsync();
            await Context.Message.DeleteAsync();
        }

        [Command("psn"), Summary("Set your PSN ID.")]
        public async Task PSN([Remainder]string psn = "")
        {
            UserAccount acc = UserManager.GetAccount(Context.User);
            var profile = acc.GetUserProfile();
            profile.psn = psn;
            UserManager.SaveAccounts();

            var msg = await Context.Channel.SendMessageAsync($"PlayStation Network ID set to `{(psn == "" ? "None" : psn)}` succesfully.");
            await Task.Delay(2500);
            await msg.DeleteAsync();
            await Context.Message.DeleteAsync();
        }

        [Command("xbox"), Summary("Set your XBox GamerTag.")]
        public async Task XBox([Remainder]string xbox = "")
        {
            UserAccount acc = UserManager.GetAccount(Context.User);
            var profile = acc.GetUserProfile();
            profile.xbox = xbox;
            UserManager.SaveAccounts();

            var msg = await Context.Channel.SendMessageAsync($"XBox GamerTag set to `{(xbox == "" ? "None" : xbox)}` succesfully.");
            await Task.Delay(2500);
            await msg.DeleteAsync();
            await Context.Message.DeleteAsync();
        }

        [Command("hobbies"), Summary("Set your profile hobbies here.")]
        public async Task Hobbies([Remainder]string hobbies = "")
        {
            UserAccount acc = UserManager.GetAccount(Context.User);
            var profile = acc.GetUserProfile();
            profile.hobbies = hobbies;
            UserManager.SaveAccounts();

            var msg = await Context.Channel.SendMessageAsync($"Hobbies {(hobbies == "" ? "`rested`" : "`changed`" )} succesfully.");
            await Task.Delay(2500);
            await msg.DeleteAsync();
            await Context.Message.DeleteAsync();
        }

        [Command("aboutme"), Summary("Set your profile description/bio here.")]
        public async Task AboutMe([Remainder]string aboutme = "")
        {
            UserAccount acc = UserManager.GetAccount(Context.User);
            var profile = acc.GetUserProfile();
            profile.aboutme = aboutme;
            UserManager.SaveAccounts();

            var msg = await Context.Channel.SendMessageAsync($"About Me description {(aboutme == "" ? "`rested`" : "`changed`")} succesfully.");
            await Task.Delay(2500);
            await msg.DeleteAsync();
            await Context.Message.DeleteAsync();
        }

        [Command("help")]
        public async Task Help([Remainder]string type = "")
        {
            if (type.ToLowerInvariant() == "profile")
            {
                string h = "Use `.view <user>` to view someone's profile or your own profile. You can add info to your profile with the following commands:\n\n" +
                           "- `.location <location>`\n" +
                           "- `.telegram <Telegram username>`\n" +
                           "- `.furaffinity <FA username>`\n" +
                           "- `.battlenet <BattleNet ID>`\n" +
                           "- `.steam <Steam profile ID for the steam link>`\n" +
                           "- `.psn <PlayStation Network ID>`\n" +
                           "- `.xbox <XBox GamerTag>`\n" +
                           "- `.aboutme <Description of yourself and what you do etc stuff you wanna put here, sfw>`";

                try
                {
                    var dms = await Context.User.GetOrCreateDMChannelAsync();
                    await dms.SendMessageAsync(h);
                }
                catch
                {
                    var error = await Context.Channel.SendMessageAsync($"{Context.User.Mention} Looks like your dms are disabled for me :(.");
                    await Task.Delay(2500);
                    await error.DeleteAsync();
                    await Context.Message.DeleteAsync();
                    return;
                }

                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} Command list sent to your dms :3.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
            }
            else if (type.ToLowerInvariant() == "fursona" || type.ToLowerInvariant() == "sona" || type.ToLowerInvariant() == "sonas")
            {
                string h = "- `.addsona <name>` : Use this to add a sona, for example, `.addsona George` would add a new sona with the name George. You can have up to 5 sonas.\n\n" +
                           "- `.editsona <name> <value to edit> <value>` : Use this to edit all values for the sona except weight and height. Values that you can put in <value to edit> are: " +
                           "name, species, gender, age, bodytype, clothing, personality, biography. For example, to change the species it would be, `.editsona George species Feline`.\n" +
                           "- `.editsonaweight <name> <value> <units>` : Change your fursona's weight, putting kg or lbs in <units>. For example: `.editsonaweight George 170 lbs`.\n" +
                           "- `.editsonaheight <name> <value> <units>` : Change your fursona's height, putting cm or in in <units> (in for inches). For example, `.editsona George 72 in`.\n" +
                           "- `.viewsona <user> <name>` : Views a user's sona, for example, to view George, you would have to do `.viewsona @user George`.\n" +
                           "- `.removesona <name>` : Removes your fursona with <name>. For example, `.removesona George`.";

                try
                {
                    var dms = await Context.User.GetOrCreateDMChannelAsync();
                    await dms.SendMessageAsync(h);
                }
                catch
                {
                    var error = await Context.Channel.SendMessageAsync($"{Context.User.Mention} Looks like your dms are disabled for me :(.");
                    await Task.Delay(2500);
                    await error.DeleteAsync();
                    await Context.Message.DeleteAsync();
                    return;
                }

                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} Command list sent to your dms :3.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();

            }
            else
            {
                string h = "Heya! use `.help profile` to see help regarding your profile editing, and `.help fursona` to get help to edit your fursona.";

                try
                {
                    var dms = await Context.User.GetOrCreateDMChannelAsync();
                    await dms.SendMessageAsync(h);
                }
                catch
                {
                    var error = await Context.Channel.SendMessageAsync($"{Context.User.Mention} Looks like your dms are disabled for me :(.");
                    await Task.Delay(2500);
                    await error.DeleteAsync();
                    await Context.Message.DeleteAsync();
                    return;
                }

                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} Command list sent to your dms :3.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();

            }

        }

        [Command("removesona"), Summary("Remove your fursona from your profile")]
        public async Task RemoveSona(string sona)
        {
            UserAccount acc = UserManager.GetAccount(Context.User);
            var sonas = acc.GetFursonas();

            if (sonas.Count == 0)
            {
                var error = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you don't have any sona's to be removed.");
                await Task.Delay(2500);
                await error.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            sonas.Remove(sona.ToLowerInvariant());
            UserManager.SaveAccounts();

            var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} your sona `sona` has been removed succesfully!.");
            await Task.Delay(2500);
            await msg.DeleteAsync();
            await Context.Message.DeleteAsync();
        }

        [Command("viewsona"), Summary("View a user's fursona profile (including your own)")]
        public async Task ViewSona(IGuildUser user, string sona = "")
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            UserAccount acc = UserManager.GetAccount((SocketUser)user);

            string s = "";
            if (sona == "" || sona == " ")
            {
                var a = acc.GetFursonas();
                if (a.Count == 0)
                {
                    var msg = await Context.Channel.SendMessageAsync($"`{user.Username}` doesn't have any sonas.");
                    await Task.Delay(2500);
                    await msg.DeleteAsync();
                    await Context.Message.DeleteAsync();
                    return;
                }

                s = a.First().Key;
            }

            s = (sona == "") ? s : sona.ToLowerInvariant(); ;
            var fursona = acc.GetSonaData(s);
            if (fursona == null)
            {
                var msg = await Context.Channel.SendMessageAsync($"`{user.Username}` doesn't have a sona named `{sona}`.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            author.WithName(user.Username);
            author.WithIconUrl(user.GetAvatarUrl());
            embed.WithAuthor(author);

            if (fursona.imagelink != "")
                embed.WithImageUrl(fursona.imagelink);
            embed.WithColor(Color.Blue);

            var name = new EmbedFieldBuilder();
            name.WithIsInline(true);
            name.WithName("Name");
            name.WithValue(fursona.name);
            embed.AddField(name);

            if (fursona.species != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Species");
                f.WithValue(fursona.species);
                embed.AddField(f);
            }

            if (fursona.gender != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Gender");
                f.WithValue(fursona.gender);
                embed.AddField(f);
            }

            if (fursona.age != 0)
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Age");
                f.WithValue($"{(int)fursona.age} years old");
                embed.AddField(f);
            }

            if (fursona.height != 0)
            {
                double inches = fursona.height * 0.393701;
                int feet = (int)inches / 12;
                inches = (int)inches % 12;

                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Height");
                f.WithValue($"{fursona.height}cm or {feet}ft {inches}");
                embed.AddField(f);
            }

            if (fursona.weight != 0)
            {
                int pounds = (int)(fursona.weight * 2.20462);

                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Weight");
                f.WithValue($"{fursona.weight} kg or {pounds} lbs");
                embed.AddField(f);
            }

            if (fursona.bodytype != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(false);
                f.WithName("BodyType");
                f.WithValue(fursona.bodytype);
                embed.AddField(f);
            }

            if (fursona.clothing != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(false);
                f.WithName("Clothing");
                f.WithValue(fursona.clothing);
                embed.AddField(f);
            }

            if (fursona.personality != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(false);
                f.WithName("Personality");
                f.WithValue(fursona.personality);
                embed.AddField(f);
            }

            if (fursona.biography != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(false);
                f.WithName("Biography");
                f.WithValue(fursona.biography);
                embed.AddField(f);
            }

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("postsona"), Summary("View a user's fursona profile (including your own)")]
        public async Task PostSona(IGuildUser user, string sona = "")
        {
            if (!RoleManager.HasModRole((SocketGuildUser)Context.User))
                return;

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            UserAccount acc = UserManager.GetAccount((SocketUser)user);

            string s = "";
            if (sona == "" || sona == " ")
            {
                var a = acc.GetFursonas();
                if (a.Count == 0)
                {
                    var msg = await Context.Channel.SendMessageAsync($"`{user.Username}` doesn't have any sonas.");
                    await Task.Delay(2500);
                    await msg.DeleteAsync();
                    await Context.Message.DeleteAsync();
                    return;
                }

                s = a.First().Key;
            }

            s = (sona == "") ? s : sona.ToLowerInvariant(); ;
            var fursona = acc.GetSonaData(s);
            if (fursona == null)
            {
                var msg = await Context.Channel.SendMessageAsync($"`{user.Username}` doesn't have a sona named `{sona}`.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            author.WithName(user.Username);
            author.WithIconUrl(user.GetAvatarUrl());
            embed.WithAuthor(author);

            if (fursona.imagelink != "")
                embed.WithImageUrl(fursona.imagelink);
            embed.WithColor(Color.Blue);

            var name = new EmbedFieldBuilder();
            name.WithIsInline(true);
            name.WithName("Name");
            name.WithValue(fursona.name);
            embed.AddField(name);

            if (fursona.species != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Species");
                f.WithValue(fursona.species);
                embed.AddField(f);
            }

            if (fursona.gender != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Gender");
                f.WithValue(fursona.gender);
                embed.AddField(f);
            }

            if (fursona.age != 0)
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Age");
                f.WithValue($"{(int)fursona.age} years old");
                embed.AddField(f);
            }

            if (fursona.height != 0)
            {
                double inches = fursona.height * 0.393701;
                int feet = (int)inches / 12;
                inches = (int)inches % 12;

                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Height");
                f.WithValue($"{fursona.height}cm or {feet}ft {inches}");
                embed.AddField(f);
            }

            if (fursona.weight != 0)
            {
                int pounds = (int)(fursona.weight * 2.20462);

                var f = new EmbedFieldBuilder();
                f.WithIsInline(true);
                f.WithName("Weight");
                f.WithValue($"{fursona.weight} kg or {pounds} lbs");
                embed.AddField(f);
            }

            if (fursona.bodytype != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(false);
                f.WithName("BodyType");
                f.WithValue(fursona.bodytype);
                embed.AddField(f);
            }

            if (fursona.clothing != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(false);
                f.WithName("Clothing");
                f.WithValue(fursona.clothing);
                embed.AddField(f);
            }

            if (fursona.personality != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(false);
                f.WithName("Personality");
                f.WithValue(fursona.personality);
                embed.AddField(f);
            }

            if (fursona.biography != "")
            {
                var f = new EmbedFieldBuilder();
                f.WithIsInline(false);
                f.WithName("Biography");
                f.WithValue(fursona.biography);
                embed.AddField(f);
            }

            var channel = ChannelManager.GetTextChannel("🦄 Roleplay", "🐾-fursonas");

            var messages = await channel.GetMessagesAsync().FlattenAsync();
            foreach (var msg in messages)
            {
                if (!msg.Content.Contains(user.Mention))
                    continue;

                foreach (var e in msg.Embeds)
                    foreach (var f in e.Fields)
                        if (f.Value.Contains(fursona.name))
                            await msg.DeleteAsync();
            }

            await channel.SendMessageAsync($"{user.Mention}", false, embed.Build());
        }

        [Command("addsona")]
        public async Task AddSona(string sona)
        {
            UserAccount acc = UserManager.GetAccount(Context.User);
            var sonas = acc.GetFursonas();

            if (sonas.Count > 5)
            {
                var error = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you already reach the max capacity of 5 fursonas.");
                await Task.Delay(2500);
                await error.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            SonaData sonaData = new SonaData();
            sonaData.name = sona;
            sonas.Add(sona.ToLowerInvariant(), sonaData);
            UserManager.SaveAccounts();

            var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you have added `{sona}` succesfully!.");
            await Task.Delay(2500);
            await msg.DeleteAsync();
            await Context.Message.DeleteAsync();
        }

        [Command("editsona")]
        public async Task EditSona(string sona, string type, [Remainder]string value = "")
        {
            UserAccount acc = UserManager.GetAccount(Context.User);
            var sonaData = acc.GetSonaData(sona.ToLowerInvariant());
            if (sonaData == null)
            {
                var error = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you don't have a sona called {sona}.");
                await Task.Delay(2500);
                await error.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            if (type.ToLowerInvariant() == "name")
            {
                sonaData.name = value;
                var sonas = acc.GetFursonas();
                sonas.Remove(sona.ToLowerInvariant());
                sonas.Add(value.ToLowerInvariant(), sonaData);
                UserManager.SaveAccounts();
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you have changed `{sona}`'s name to `{value}`succesfully!.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            if (type.ToLowerInvariant() == "species")
            {
                sonaData.species = value;
                UserManager.SaveAccounts();
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you have added `{value}` to `{sona}`succesfully!.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            if (type.ToLowerInvariant() == "gender")
            {
                sonaData.gender = value;
                UserManager.SaveAccounts();
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you have added `{value}` to `{sona}`succesfully!.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            if (type.ToLowerInvariant() == "age")
            {
                sonaData.age = int.Parse(value);
                UserManager.SaveAccounts();
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you have added `{value}` years old to `{sona}`succesfully!.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            if (type.ToLowerInvariant() == "height")
            {
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} use `.editsonaheight <sona> <height value> <units (cm or in)>` for that.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            if (type.ToLowerInvariant() == "weight")
            {
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} use `.editsonaweight <sona> <weight value> <units (kg or lbs)>` for that.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            if (type.ToLowerInvariant() == "bodytype")
            {
                sonaData.bodytype = value;
                UserManager.SaveAccounts();
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you have added `bodytype` to `{sona}`succesfully!.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            if (type.ToLowerInvariant() == "clothing")
            {
                sonaData.clothing = value;
                UserManager.SaveAccounts();
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you have added `clothing` to `{sona}`succesfully!.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            if (type.ToLowerInvariant() == "personality")
            {
                sonaData.personality = value;
                UserManager.SaveAccounts();
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you have added `personality` to `{sona}`succesfully!.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            if (type.ToLowerInvariant() == "biography")
            {
                sonaData.biography = value;
                UserManager.SaveAccounts();
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you have added `biography` to `{sona}`succesfully!.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            if (type.ToLowerInvariant() == "imagelink" || type.ToLowerInvariant() == "image")
            {
                sonaData.imagelink = value;
                UserManager.SaveAccounts();
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you have added `imagelink` to `{sona}`succesfully!.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }
        }

        [Command("editsonaweight")]
        public async Task EditSonaWeight(string sona, int weight, string units)
        {
            UserAccount acc = UserManager.GetAccount(Context.User);
            var sonaData = acc.GetSonaData(sona.ToLowerInvariant());
            if (sonaData == null)
            {
                var error = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you don't have a sona called {sona}.");
                await Task.Delay(2500);
                await error.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            if (units.ToLowerInvariant() == "kg" || units.ToLowerInvariant() == "kgs")
            {
                sonaData.weight = weight;
                UserManager.SaveAccounts();
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you have added `{weight} kgs` weight to `{sona}`succesfully!.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }
            else if (units.ToLowerInvariant() == "lbs" || units.ToLowerInvariant() == "lb")
            {
                sonaData.weight = (int)(weight / 2.205);
                UserManager.SaveAccounts();
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you have added `{weight} lbs` weight to `{sona}`succesfully!.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }
            else
            {
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} use `kg` or `lbs` for the <units>.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }
        }

        [Command("editsonaheight")]
        public async Task EditSonaHeight(string sona, int height, string units)
        {
            UserAccount acc = UserManager.GetAccount(Context.User);
            var sonaData = acc.GetSonaData(sona.ToLowerInvariant());
            if (sonaData == null)
            {
                var error = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you don't have a sona called {sona}.");
                await Task.Delay(2500);
                await error.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            if (units.ToLowerInvariant() == "cm" || units.ToLowerInvariant() == "cms")
            {
                sonaData.height = height;
                UserManager.SaveAccounts();
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you have added `{height} cms` height to `{sona}`succesfully!.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }
            else if (units.ToLowerInvariant() == "in" || units.ToLowerInvariant() == "ins")
            {
                int feet = (int)(height / 12);
                int inches = (int)(height % 12);

                sonaData.height = (int)(height * 2.54);
                UserManager.SaveAccounts();
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} you have added `{feet} ft {inches}` height to `{sona}`succesfully!.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }
            else
            {
                var msg = await Context.Channel.SendMessageAsync($"{Context.User.Mention} use `cm` or `in` for the <units>.");
                await Task.Delay(2500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }
        }

        [Command("userinfo")]
        [Summary("Views the user's info, leaving it blank will show the default info.")]
        public async Task UserInfo(IGuildUser user, [Remainder]string type = "")
        {
            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                var embed = new EmbedBuilder();
                var author = new EmbedAuthorBuilder();
                UserAccount acc = UserManager.GetAccount((SocketUser)user);

                author.WithName(user.Username);
                author.WithIconUrl(user.GetAvatarUrl());
                embed.WithAuthor(author);

                embed.WithThumbnailUrl(user.GetAvatarUrl());
                embed.WithColor(Color.Blue);

                if (type == "")
                {
                    var f1 = new EmbedFieldBuilder();
                    f1.WithIsInline(true);
                    f1.WithName("Joined At");
                    f1.WithValue($"{user.JoinedAt.Value.Day} {Utilities.GetMonth(user.JoinedAt.Value.Month)} {user.JoinedAt.Value.Year}");
                    embed.AddField(f1);

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
                else if (type.ToLowerInvariant() == "warnings")
                {
                    embed.WithTitle("Warnings");
                    string s = "";
                    if (acc.modData.WarnCount() == 0)
                        s = "This user has no warnings.";
                    else
                        for (int i = 0; i < acc.modData.WarnCount(); ++i)
                        {
                            s += $"**{i + 1}** {Context.Guild.GetUser(acc.modData.warnings[i].staff).Mention} ({acc.modData.warnings[i].date}) : `{acc.modData.warnings[i].reason}`.";
                            s += i < acc.modData.WarnCount() ? "\n" : "";
                        }

                    embed.WithDescription(s);
                }
                else if (type.ToLowerInvariant() == "mutes")
                {
                    embed.WithTitle("Mutes");
                    string s = "";
                    if (acc.modData.MuteCount() == 0)
                        s = "This user hasn't been muted.";
                    else
                        for (int i = 0; i < acc.modData.MuteCount(); ++i)
                        {
                            s += $"**{i + 1}** {Context.Guild.GetUser(acc.modData.mutes[i].staff).Mention} ({acc.modData.mutes[i].date}) for ({acc.modData.mutes[i].time}): `{acc.modData.mutes[i].reason}`.";
                            s += i < acc.modData.MuteCount() ? "\n" : "";
                        }

                    embed.WithDescription(s);
                }
                else if (type.ToLowerInvariant() == "kicks")
                {
                    embed.WithTitle("Kicks");
                    string s = "";
                    if (acc.modData.KickCount() == 0)
                        s = "This user hasn't been kicked.";
                    else
                        for (int i = 0; i < acc.modData.KickCount(); ++i)
                        {
                            s += $"**{i + 1}** {Context.Guild.GetUser(acc.modData.kicks[i].staff).Mention} ({acc.modData.kicks[i].date}) : `{acc.modData.kicks[i].reason}`.";
                            s += i < acc.modData.KickCount() ? "\n" : "";
                        }

                    embed.WithDescription(s);
                }
                else if (type.ToLowerInvariant() == "softbans")
                {
                    embed.WithTitle("Softbans");
                    string s = "";
                    if (acc.modData.SoftBanCount() == 0)
                        s = "This user hasn't been softbanned.";
                    else
                        for (int i = 0; i < acc.modData.SoftBanCount(); ++i)
                        {
                            s += $"**{i + 1}** {Context.Guild.GetUser(acc.modData.softBans[i].staff).Mention} ({acc.modData.softBans[i].date}) : `{acc.modData.softBans[i].reason}`.";
                            s += i < acc.modData.SoftBanCount() ? "\n" : "";
                        }

                    embed.WithDescription(s);
                }
                else if (type.ToLowerInvariant() == "bans")
                {
                    embed.WithTitle("Bans");
                    string s = "";
                    if (acc.modData.BanCount() == 0)
                        s = "This user hasn't been banned.";
                    else
                        for (int i = 0; i < acc.modData.BanCount(); ++i)
                        {
                            s += $"**{i + 1}** {Context.Guild.GetUser(acc.modData.bans[i].staff).Mention} ({acc.modData.bans[i].date}) : `{acc.modData.bans[i].reason}`.";
                            s += i < acc.modData.BanCount() ? "\n" : "";
                        }

                    embed.WithDescription(s);
                }

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }
    }
}
