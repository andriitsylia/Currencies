using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using TelegramBot.Constants;
using TelegramBot.Handlers;

namespace TelegramBot.Services
{
    public class Bot
    {
        private static string _token;

        public Bot(string token)
        {
            _token = token;
        }

        public static async Task Run()
        {
            ITelegramBotClient botClient = new TelegramBotClient(_token);
            using CancellationTokenSource cts = new();

            User me = await botClient.GetMeAsync();
            Console.Title = me.Username ?? BotInfoMessage.BOT_CONSOLE_TITLE;

            ReceiverOptions receiverOption = new()
            {
                AllowedUpdates = { },
                ThrowPendingUpdates = true
            };
            botClient.StartReceiving(MainHandler.HandleUpdateAsync, MainHandler.HandleErrorAsync, receiverOption, cts.Token);

            Console.WriteLine(BotInfoMessage.BOT_LISTENING + me.Username);
            Console.ReadLine();

            cts.Cancel();
        }
    }
}
