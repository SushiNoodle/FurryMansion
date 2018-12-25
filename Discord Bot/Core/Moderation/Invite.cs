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

namespace Discord_Bot.Core.Moderation
{
    public class InviteCommands : ModuleBase<SocketCommandContext>
    {
        [Command("invite")]
        public async Task Invite(IGuildUser user = null)
        {
            if (user == null)
                return;

            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                var welcome = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "🛎-welcome-hall");
                var invite = await welcome.CreateInviteAsync(86400, 1, true, true);

                try
                {
                    var dms = await user.GetOrCreateDMChannelAsync();
                    await dms.SendMessageAsync($"Your invite link is: {invite.Url}");
                    var msg = await Context.Channel.SendMessageAsync($"Link sent to {user.Mention}'s dms.");
                    await Task.Delay(1500);
                    await msg.DeleteAsync();
                    await Context.Message.DeleteAsync();
                }
                catch
                {
                    var msg = await Context.Channel.SendMessageAsync($"Error! couldn't get {user.Mention}'s dms.");
                    await Task.Delay(1500);
                    await msg.DeleteAsync();
                    await Context.Message.DeleteAsync();
                }

            }
        }
    }
}
