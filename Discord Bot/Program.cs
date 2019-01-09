using Discord.WebSocket;
using Discord;
using System;
using System.Threading.Tasks;
using Discord_Bot.Core;
using Discord_Bot.Modules.Role_System;
using Discord_Bot.Modules.Channel_System;
using Discord.Rest;
using Discord_Bot.Modules.Twitter;
using Discord_Bot.Core.Data;
using Discord_Bot.Core.Moderation;
using Discord_Bot.Modules.Logging_System;
using Discord_Bot.Modules.Voting_System;

namespace Discord_Bot
{
    class Program
    {
        static public DiscordSocketClient _client;
        static public CommandHandler _handler;

        // Async discord start.
        static void Main(string[] args)
        => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            if (Config.bot.token == "" | Config.bot.token == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 100

            });
            _client.Log += Logger;
            _client.Connected += _client_Connected;

            _client.ChannelCreated += _client_ChannelCreated;
            _client.ChannelDestroyed += _client_ChannelDestroyed;
            _client.ChannelUpdated += _client_ChannelUpdated;

            _client.GuildUpdated += _client_GuildUpdated;

            _client.MessageReceived += _client_MessageReceived;
            _client.MessageDeleted += _client_MessageDeleted;

            _client.UserJoined += _client_UserJoined;
            _client.UserLeft += _client_UserLeft;
            _client.UserBanned += _client_UserBanned;
            _client.UserUnbanned += _client_UserUnbanned;

            _client.RoleCreated += _client_RoleCreated;
            _client.RoleDeleted += _client_RoleDeleted;
            _client.RoleUpdated += _client_RoleUpdated;

            _client.Ready += RepeatingTimer.StartTimer;
            _client.Ready += _client_Ready;

            _client.ReactionAdded += OnReactionAdded;

            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.StartAsync();

            Global.Client = _client;

            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);

            // StreamLabelsManager.Connect();
            // TwitchTvUtilities.Connect();
            TwitterManager.Connect();

            await Task.Delay(-1);
        }

        private async Task _client_Ready()
        {
            var guild = _client.GetGuild(494596551394721828);
            var office = guild.GetTextChannel(496875452166701066);
            await office.SendMessageAsync("Hewwo uwu I just came online >.< sorry I'm kinda shy but if you need anything just dm me ;3 uwu ^//^.");
        }

        public async Task Logger(LogMessage lmsg)
        {
            switch (lmsg.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
            }

            Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | [Discord] : {lmsg.Message}");
            Console.ResetColor();
        }

        private async Task _client_MessageReceived(SocketMessage arg)
        {
            if (arg.Channel.Name == "role-picker" || arg.Channel.Name == "role-picker-18")
            {
                await Task.Delay(2500);
                await arg.DeleteAsync();
            }
        }

        private async Task _client_MessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            var msg = await arg1.GetOrDownloadAsync();

            if (msg.Channel.Name == "🛎-welcome-hall")
            {
                var logs = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "📋-joins-and-leaves");
                var embed = new EmbedBuilder();
                var author = new EmbedAuthorBuilder();
                var footer = new EmbedFooterBuilder();

                author.WithName("Message Deleted");
                author.WithIconUrl(msg.Author.GetAvatarUrl());
                embed.WithAuthor(author);

                footer.WithText($"ID: {msg.Author.Id}");
                embed.WithFooter(footer);

                embed.WithCurrentTimestamp();
                embed.WithColor(Color.Red);
                embed.WithDescription(msg.Content);

                await logs.SendMessageAsync("", false, embed.Build());
            }
        }

        private async Task _client_UserJoined(SocketGuildUser arg)
        {
            var channel = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "🛎-welcome-hall");

            await channel.SendMessageAsync($"Welcome to **Furry Mansion** {arg.Mention}! :tada: \n" +
                                  $"Read the rules in {ChannelManager.GetTextChannel("📌 Bulletin Board", "🔨-rules-and-faq").Mention}, " +
                                  $"a {RoleManager.GetRole("Welcomer").Mention} will verify you soon. \n" +
                                  $"If you fail to get verified in 30 minutes you'll be kicked out.");

            await arg.AddRoleAsync(await RoleManager.GetOrCreateRoleAsync($"{Utilities.GetMonth()} {DateTime.Today.Year}"));
            Core.Moderation.Verification.Add(arg);

            LoggingManager.LogUserJoined(arg);
        }

        private async Task _client_UserBanned(SocketUser arg1, SocketGuild arg2)
        {
            Verification.Remove((SocketGuildUser)arg1);
            LoggingManager.LogUserBanned(arg1, arg2);
        }

        private async Task _client_UserUnbanned(SocketUser arg1, SocketGuild arg2)
        {
            LoggingManager.LogUserUnBanned(arg1, arg2);
        }

        private async Task _client_UserLeft(SocketGuildUser arg)
        {
            LoggingManager.LogUserLeft(arg);
        }

        private async Task _client_GuildUpdated(SocketGuild arg1, SocketGuild arg2)
        {
            RoleManager.Initialize();
            ChannelManager.Initialize();
        }

        private async Task _client_RoleUpdated(SocketRole arg1, SocketRole arg2)
        {
            RoleManager.Initialize();
        }

        private async Task _client_RoleDeleted(SocketRole arg)
        {
            RoleManager.Initialize();
        }

        private async Task _client_RoleCreated(SocketRole arg)
        {
            RoleManager.Initialize();
        }

        private async Task _client_ChannelUpdated(SocketChannel arg1, SocketChannel arg2)
        {
            ChannelManager.Initialize();
        }

        private async Task _client_ChannelDestroyed(SocketChannel arg)
        {
            ChannelManager.Initialize();
        }

        private async Task _client_ChannelCreated(SocketChannel arg)
        {
            ChannelManager.Initialize();
        }

        private async Task _client_Connected()
        {
            await _client.SetGameAsync("Voring Furries");
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.Emote.Name == "POGGERS")
            {
                var rMessage = (RestUserMessage)await channel.GetMessageAsync(reaction.MessageId);
                await rMessage.AddReactionAsync(ChannelManager.GetEmote("POGGERS"));
            }

            StaffVoting.HandleStaffVote(reaction);
            NSFWVoting.HandleVote(reaction);

        }
    }
}
