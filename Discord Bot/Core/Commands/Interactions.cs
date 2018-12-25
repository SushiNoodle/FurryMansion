using Discord;
using Discord.Commands;
using Discord_Bot.Modules.Channel_System;
using System.Threading.Tasks;

namespace Discord_Bot.Core.Commands
{
    public class Interactions : ModuleBase<SocketCommandContext>
    {
        [Command("bap")]
        public async Task Bap(IGuildUser user)
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync($"{Context.User.Mention} baps {user.Mention} furiously :anger:");
        }


        [Command("hug")]
        public async Task Hug(IGuildUser user)
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync($"{Context.User.Mention} hugs {user.Mention} nice and tight {ChannelManager.GetEmote("tHug")}");
        }

        [Command("boop")]
        public async Task Boop(IGuildUser user)
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync($"{user.Mention} just got booped by {Context.User.Mention} {ChannelManager.GetEmote("tBlush")}");
        }
    }
}