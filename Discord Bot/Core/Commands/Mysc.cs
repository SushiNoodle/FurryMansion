using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using NReco.ImageGenerator;

namespace Discord_Bot.Modules
{
    public class Mysc : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        public async Task Echo([Remainder]string message)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Message by " + Context.User.Username);
            embed.WithDescription(message);
            embed.WithColor(new Color(0, 255, 0));

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        // Testing image generating.
        [Command("hello")]
        public async Task Hello(string color = "red")
        {
            string css = "<style>\n   h1{\n       color: " + color + ";\n    }\n</style>\n";
            string html = String.Format("<h1>Hello {0}!</h1>", Context.User.Username);
            var converter = new HtmlToImageConverter
            {
                Width = 250,
                Height = 70
            };
            var jpgBytes = converter.GenerateImage(css + html, NReco.ImageGenerator.ImageFormat.Jpeg);
            await Context.Channel.SendFileAsync(new MemoryStream(jpgBytes), "hello.jpeg");
        }

        // Picks one of the choices given seperated by | by random.
        [Command("pick")]
        public async Task Pick([Remainder]string message)
        {
            string[] options = message.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            Random r = new Random();
            string selection = options[r.Next(0, options.Length)];

            var embed = new EmbedBuilder();

            if (options.Length == 1)
            {
                embed.WithTitle("Thats Cheating!");
                embed.WithDescription("Give me more options to pick from. :triumph:");
                embed.WithColor(new Color(255, 0, 0));
            }
            else
            {
                embed.WithTitle("Choice for " + Context.User.Username);
                embed.WithDescription(selection);
                embed.WithColor(new Color(61, 182, 44));
            }

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
