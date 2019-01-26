using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Discord_Bot.Core.Data;
using Discord_Bot.Modules.Channel_System;
using Discord_Bot.Modules.Logging_System;
using Discord_Bot.Modules.Role_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Core.Moderation
{
    public static class ToDoList
    {
        public static IUserMessage ToDoMessage;
        public static List<string> toDoList;
        private static string todolistFile = "Resources/todoList.json";

        static ToDoList()
        {
            if (DataStorage.SaveExists(todolistFile))
                toDoList = DataStorage.LoadList<string>(todolistFile);
            else
            {
                toDoList = new List<string>();
                SaveList();
            }

            ToDoMessage = null;
        }

        public static void SaveList()
        {
            DataStorage.SaveList(toDoList, todolistFile);
        }

        public static List<string> GetList()
        {
            return toDoList;
        }

        public static string PrintList()
        {
            try
            {
                string result = "";
                for (int i = 0; i < toDoList.Count; ++i)
                {
                    if (i != 0)
                        result += "\n";
                    result += $"**{i + 1}** : {toDoList[i]}";
                }

                return result;
            }
            catch
            {
                return "";
            }

        }

        public static async void ClearLists()
        {
            var office = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "🚬-ty’s-office");

            var bot = office.GetUser(Global.DiscordBotID);
            var messages = await office.GetMessagesAsync(100).FlattenAsync();

            foreach (var msg in messages)
            {
                if (msg.Author.Id != bot.Id)
                    continue;

                if (msg.IsPinned)
                {
                    if (ToDoMessage != null && msg.Id == ToDoMessage.Id)
                        continue;

                    var result = from f in msg.Embeds
                                 where f.Title == "Ty's To Do List"
                                 select f;

                    if (result.FirstOrDefault() == null)
                        continue;

                    await msg.DeleteAsync();
                }
            }
        }

        public static async void Post()
        {
            if (toDoList.Count == 0)
                return;

            var office = ChannelManager.GetTextChannel("🏰 Ty's Mansion", "🚬-ty’s-office");

            if (ToDoMessage != null)
            {
                try { await ToDoMessage.DeleteAsync(); }
                catch { ToDoMessage = null; }
            }

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            var bot = Global.Client.GetUser(Global.DiscordBotID);

            embed.WithTitle("Ty's To Do List");
            embed.WithCurrentTimestamp();

            author.WithIconUrl(bot.GetAvatarUrl());
            author.WithName(bot.Username);

            embed.WithAuthor(author);
            embed.WithDescription(PrintList());

            ToDoMessage = await office.SendMessageAsync("", false, embed.Build());
            await ToDoMessage.PinAsync();
        }

        public static void Add(string element)
        {
            toDoList.Add(element);
            Post();
        }

        public static bool Remove(int index)
        {
            if (toDoList.Count == 0 || index > toDoList.Count)
                return false;

            toDoList.RemoveAt(index - 1);
            Post();

            return true;
        }

    }

    public class ToDoListCommands : ModuleBase<SocketCommandContext>
    {
        [Command("list")]
        public async Task List(string type = "", [Remainder]string value = "")
        {
            if (RoleManager.HasAdminRole((SocketGuildUser)Context.User))
            {
                if (type == "" || type.ToLowerInvariant() == "post")
                {
                    ToDoList.ClearLists();
                    ToDoList.Post();
                }

                else if (type.ToLowerInvariant() == "add")
                {
                    ToDoList.ClearLists();
                    ToDoList.Add(value);
                    ToDoList.SaveList();
                }

                else if (type.ToLowerInvariant() == "remove")
                {
                    ToDoList.ClearLists();
                    ToDoList.Remove(int.Parse(value));
                    ToDoList.SaveList();
                }
            }
        }
    }
}
