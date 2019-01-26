using Discord.Commands;
using Discord.WebSocket;
using Discord_Bot.Core.Data;
using Discord_Bot.Modules.Role_System;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord_Bot.Core.Commands
{
    internal class JokeManager
    {
        private static List<string> jokes;
        private static Random rnd;
        private static string jokesFile = "Resources/jokes.json";

        static JokeManager()
        {
            if (DataStorage.SaveExists(jokesFile))
                jokes = DataStorage.LoadList<string>(jokesFile);
            else
            {
                jokes = new List<string>();
                SaveJokes();
            }

            rnd = new Random();
        }

        internal static void SaveJokes()
        {
            DataStorage.SaveList(jokes, jokesFile);
        }

        internal static string GetRandomJoke()
        {
            return jokes[rnd.Next(jokes.Count)];
        }

        internal static void AddJoke(string joke)
        {
            jokes.Add(joke);
        }
    }

    public class JokeCommands : ModuleBase<SocketCommandContext>
    {
        [Command("joke")]
        public async Task Joke()
        {
            await Context.Channel.SendMessageAsync(JokeManager.GetRandomJoke());
            await Context.Message.DeleteAsync();
        }

        [Command("joke")]
        public async Task Joke(string option, [Remainder]string value)
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                if (option.ToLowerInvariant() == "add")
                {
                    JokeManager.AddJoke(value);
                    JokeManager.SaveJokes();
                    var msg = await Context.Channel.SendMessageAsync("Joke has been added correctly to the bot!");
                    await Task.Delay(2500);
                    await msg.DeleteAsync();
                    await Context.Message.DeleteAsync();
                }
            }
        }
    }
}
