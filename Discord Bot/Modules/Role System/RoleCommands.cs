using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Bot.Modules.Channel_System;
using System;
using System.Threading.Tasks;

namespace Discord_Bot.Modules.Role_System
{
    public class RoleCommands : ModuleBase<SocketCommandContext>
    {
        [Command("iam")]
        [Summary("Grants self assignable role to user.")]
        public async Task IAm([Remainder]string role)
        {
            if (Context.Channel.Name != "role-picker" && Context.Channel.Name != "role-picker-18")
            {
                var roleChannel = ChannelManager.GetTextChannel("🤖 Skynet", "role-picker");
                await roleChannel.SendMessageAsync($"{Context.User.Mention} please use this channel for that.");
                await Task.Delay(1000);
                await Context.Message.DeleteAsync();
                return;
            }

            var r = RoleManager.GetSelfAssignable(role);
            var r18 = RoleManager.GetSelfAssignable18(role);

            if (r == "" && r18 == "")
                await Context.Channel.SendMessageAsync($"**{role}** doesn't exist or isn't self assignable.");
            else if (r18 != "")
            {
                string[] roles18 = { "+18", "+18 (no lewd)" };
                if (RoleManager.CheckUserRole((SocketGuildUser)Context.User, roles18))
                {
                    await ((SocketGuildUser)Context.User).AddRoleAsync(RoleManager.GetRole(r18));
                    await Context.Channel.SendMessageAsync($"**{r18}** has been added succesfully!");
                }
                else
                    await Context.Channel.SendMessageAsync($"**{r18}** can't be added since you don't have the `+18+ or `+18 (no lewd)` role.");
            }
            else if (r != "")
            {
                await ((SocketGuildUser)Context.User).AddRoleAsync(RoleManager.GetRole(r));
                await Context.Channel.SendMessageAsync($"**{r}** has been added succesfully!");
            }
        }

        [Command("iamnot")]
        [Alias("iamn", "iamnt")]
        [Summary("Removes self assignable role from user.")]
        public async Task IAmNot([Remainder]string role)
        {
            if (Context.Channel.Name != "role-picker" && Context.Channel.Name != "role-picker-18")
            {
                var roleChannel = ChannelManager.GetTextChannel("🤖 Skynet", "role-picker");
                await roleChannel.SendMessageAsync($"{Context.User.Mention} please use this channel for that.");
                await Task.Delay(1000);
                await Context.Message.DeleteAsync();
                return;
            }

            var r = RoleManager.GetSelfAssignable(role);
            var r18 = RoleManager.GetSelfAssignable18(role);

            if (r == "" && r18 == "")
                await Context.Channel.SendMessageAsync($"**{role}** doesn't exist or isn't self removable.");
            else
            {
                if (r != "")
                {
                    await ((SocketGuildUser)Context.User).RemoveRoleAsync(RoleManager.GetRole(r));
                    await Context.Channel.SendMessageAsync($"**{r}** has been removed succesfully!");
                }
                else if (r18 != "")
                {
                    await ((SocketGuildUser)Context.User).RemoveRoleAsync(RoleManager.GetRole(r18));
                    await Context.Channel.SendMessageAsync($"**{r18}** has been removed succesfully!");
                }
            }
        }

        [Command("is18")]
        [Summary("Grants +18 role mentioned user, add no lewd after to grant them +18 (no lewd) role instead.")]
        public async Task Is18(IGuildUser user, [Remainder]string arg = "")
        {
            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                if (arg == "" || arg == "lewd")
                    await user.AddRoleAsync(RoleManager.GetRole("+18"));
                else if (arg == "no lewd" || arg == "non lewd")
                    await user.AddRoleAsync(RoleManager.GetRole("+18 (no lewd)"));

                var channel = ChannelManager.GetTextChannel("🔞 Nono zone", "general");
                var role_picker = ChannelManager.GetTextChannel("🤖 Skynet", "role-picker-18");
                await channel.SendMessageAsync($"Welcome {user.Mention} to the dark side! {ChannelManager.GetEmote("THONKERS")}. "
                    + $"Use the {role_picker.Mention} channel to assign your +18 roles.");
            }
        }

        [Command("isnot18")]
        [Alias("isn18", "isnt18")]
        [Summary("Removes +18 and/or +18 (no lewd) role from mentioned user.")]
        public async Task IsNot18(IGuildUser user)
        {
            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                await user.RemoveRoleAsync(RoleManager.GetRole("+18"));
                await user.RemoveRoleAsync(RoleManager.GetRole("+18 (no lewd)"));
            }
        }

        [Command("role"), Summary("Admin moderation command to manage roles easier")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task Role(IGuildUser user, [Remainder]string role)
        {
            try
            {
                await user.AddRoleAsync(RoleManager.GetRole(role));
                await Context.Channel.SendMessageAsync($"`{role}` has been added to {user.Mention} succesfully!");
            }
            catch
            {
                await Context.Channel.SendMessageAsync($"`{role}` does not exist or could not be added to {user.Mention}.");
            }

        }

        [Command("derole"), Summary("Admin moderation command to manage roles easier")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task DeRole(IGuildUser user, [Remainder]string role)
        {
            try
            {
                await user.RemoveRoleAsync(RoleManager.GetRole(role));
                await Context.Channel.SendMessageAsync($"`{role}` has been removed from {user.Mention} succesfully!");
            }
            catch
            {
                await Context.Channel.SendMessageAsync($"`{role}` does not exist or could not be removed from {user.Mention}.");
            }
        }

        [Command("Correct18Roles"), Summary("Admin command that fixes all the 18+ roles so only peeps 18+ can have em.")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task Correct18Roles()
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                var roles18 = RoleManager.SelfAssignable18;
                var users = Context.Guild.Users;
                string[] is18 = { "+18", "+18 (no lewd)" };
                bool corrected = false;

                foreach (SocketGuildUser u in users)
                {
                    if (RoleManager.CheckUserRole(u, is18))
                        continue;

                    foreach (string r in roles18)
                    {
                        if (RoleManager.CheckUserRole(u, r))
                        {
                            if (corrected == false)
                                corrected = true;

                            await u.RemoveRoleAsync(RoleManager.GetRole(r));
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | [Discord] : {r} role removed from {u.Username}");
                            Console.ResetColor();
                        }
                    }
                }

                if (corrected == true)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | [Discord] : All roles have been corrected!");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | [Discord] : No roles to correct.");
                    Console.ResetColor();
                }

            }
        }


        [Command("setselfassignable"), Summary("Sets a role to be self assignable.")]
        [Alias("setselfassign", "setselfrole")]
        public async Task SetSelfAssignable([Remainder]string role)
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                var r = RoleManager.AddSelfAssignable(role);

                if (r == null)
                    await Context.Channel.SendMessageAsync($"`{role}` role does not exist or is already self assignable.");
                else
                    await Context.Channel.SendMessageAsync($"`{role}` is now self assignable!");
            }
        }
        
        [Command("unsetselfassignable"), Summary("Unsets a role to be self assignable.")]
        [Alias("unsetselfassign", "unsetselfrole")]
        public async Task UnSetSelfAssignable([Remainder]string role)
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                var r = RoleManager.RemoveSelfAssignable(role);
        
                if (r == null)
                    await Context.Channel.SendMessageAsync($"`{role}` role does not exist or isnt self assignable.");
                else
                    await Context.Channel.SendMessageAsync($"`{role}` is not self assignable anymore!");
            }
        }

        [Command("setselfassignable18"), Summary("Sets a role to be self assignable.")]
        [Alias("setselfassign18", "setself18role")]
        public async Task SetSelfAssignable18([Remainder]string role)
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                var r = RoleManager.AddSelfAssignable18(role);

                if (r == null)
                    await Context.Channel.SendMessageAsync($"`{role}` role does not exist or is already self assignable for 18+.");
                else
                    await Context.Channel.SendMessageAsync($"`{role}` is now self assignable for 18+!");
            }
        }

        [Command("unsetselfassignable18"), Summary("Unsets a role to be self assignable.")]
        [Alias("unsetselfassign18", "unsetself18role")]
        public async Task UnSetSelfAssignable18([Remainder]string role)
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                var r = RoleManager.RemoveSelfAssignable18(role);

                if (r == null)
                    await Context.Channel.SendMessageAsync($"`{role}` role does not exist or isnt self assignable for 18+.");
                else
                    await Context.Channel.SendMessageAsync($"`{role}` is not self assignable anymore for 18+!");
            }
        }

        [Command("setmodrole"), Summary("Sets a role to be a mod role.")]
        [Alias("setmod")]
        public async Task SetMod([Remainder]string role)
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                var r = RoleManager.AddModRole(role);

                if (r == null)
                    await Context.Channel.SendMessageAsync($"`{role}` role does not exist or is already a mod role.");
                else
                    await Context.Channel.SendMessageAsync($"`{role}` is now a mod role!");
            }
        }

        [Command("unsetmodrole"), Summary("Unsets a role to be a mod role.")]
        [Alias("unsetmod")]
        public async Task UnSetMod([Remainder]string role)
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                var r = RoleManager.RemoveModRole(role);

                if (r == null)
                    await Context.Channel.SendMessageAsync($"`{role}` role does not exist or isnt a mod role.");
                else
                    await Context.Channel.SendMessageAsync($"`{role}` is not a mod role anymore!");
            }
        }

        [Command("setadminrole"), Summary("Sets a role to be a admin role.")]
        [Alias("setadmin")]
        public async Task SetAdmin([Remainder]string role)
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                var r = RoleManager.AddAdminRole(role);

                if (r == null)
                    await Context.Channel.SendMessageAsync($"`{role}` role does not exist or is already a admin role.");
                else
                    await Context.Channel.SendMessageAsync($"`{role}` is now a admin role!");
            }
        }

        [Command("unsetadminrole"), Summary("Unsets a role to be a admin role.")]
        [Alias("unsetadmin")]
        public async Task UnSetAdmin([Remainder]string role)
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                var r = RoleManager.RemoveModRole(role);

                if (r == null)
                    await Context.Channel.SendMessageAsync($"`{role}` role does not exist or isnt a admin role.");
                else
                    await Context.Channel.SendMessageAsync($"`{role}` is not a admin role anymore!");
            }
        }
    }
}
