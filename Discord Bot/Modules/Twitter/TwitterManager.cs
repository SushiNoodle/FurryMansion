using Discord_Bot.Modules.Channel_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;

namespace Discord_Bot.Modules.Twitter
{
    public static class TwitterManager
    {
        internal static async void Connect()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{ DateTime.Now.ToShortTimeString()} | [Twitter] : Connecting Twitter Bot.");
            Console.ResetColor();

            Auth.SetUserCredentials(Credentials.consumerKey, Credentials.consumerSecret, Credentials.userAccessToken, Credentials.userAccessSecret);
            var user = User.GetAuthenticatedUser();

            // Debugging.
            if (!Auth.Credentials.AreSetupForUserAuthentication())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{ DateTime.Now.ToShortTimeString()} | [Twitter] : Error! Credentials not set up for user authentication.");
                Console.ResetColor();
            }

            // Debugging.
            if (!(user == null))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{ DateTime.Now.ToShortTimeString()} | [Twitter] : Twitter account is @{user.UserIdentifier}.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | [Twitter] : Error! Twitter acccount is null.");
                Console.ResetColor();
                return;
            }

            Tweet.PublishTweet("Hello World");

            var stream = Stream.CreateUserStream();

            stream.TweetCreatedByMe += Stream_TweetCreatedByMe;

            await stream.StartStreamAsync();
        }
        
        internal static async void Stream_TweetCreatedByMe(object sender, Tweetinvi.Events.TweetReceivedEventArgs e)
        {
            var channel = ChannelManager.GetTextChannel("🐾 Plebs", "media");
            await channel.SendMessageAsync(e.Tweet.FullText);
        }
    }
}
