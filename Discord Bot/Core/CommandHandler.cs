using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Discord_Bot.Modules.Role_System;
using Discord_Bot.Core.Data;

namespace Discord_Bot.Core
{
    class CommandHandler
    {
        DiscordSocketClient _client;
        CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg)) return;

            var context = new SocketCommandContext(_client, msg);

<<<<<<< HEAD
            try
=======
            if (s.Author.IsWebhook)
                return;

            // Mute check
            if (RoleManager.CheckUserRole((SocketGuildUser)context.User, "Prisoner") &&
                context.Channel.Name != "⛓-basement-cell")
>>>>>>> origin/master
            {
                // Mute check
                if (!s.Author.IsWebhook &&
                    RoleManager.CheckUserRole((SocketGuildUser)context.User, "Prisoner") &&
                    context.Channel.Name != "⛓-basement-cell")
                {
                    await context.Message.DeleteAsync();
                    return;
                }
            }
            catch
            {

            }

            // Command handling
            int argPos = 0;
            if (msg.HasStringPrefix(Config.bot.cmdPrefix, ref argPos)
                || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos);
                if(!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}
