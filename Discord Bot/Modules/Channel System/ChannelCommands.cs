using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Discord_Bot.Modules.Role_System;
using System;
using System.Threading.Tasks;

namespace Discord_Bot.Modules.Channel_System
{
    public class ChannelCommands : ModuleBase<SocketCommandContext>
    {
        [Command("purge", RunMode = RunMode.Async)]
        [Alias("delete", "clear")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int num = 1)
        {
            if (RoleManager.HasModRole((SocketGuildUser)Context.User))
            {
                int max = num / 100 + 1;
                for (int i = 0; i < max; ++i)
                {
                    var messages = await Context.Channel.GetMessagesAsync(((num - i * 100) / 100) == 0 ? num % 100 + 1 : 100).FlattenAsync();

                    foreach (var message in messages)
                    {
                        await message.DeleteAsync();
                        await Task.Delay(100);
                    }
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[Discord] : {num} messages deleted in {Context.Channel.Name}");
                Console.ResetColor();
            }
        }
    }
}
