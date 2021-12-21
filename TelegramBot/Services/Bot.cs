using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.BotHandlers;

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
            Console.Title = me.Username ?? "Telegram currency bot";

            ReceiverOptions receiverOption = new() { AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }, ThrowPendingUpdates = true };
            botClient.StartReceiving(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync, receiverOption, cts.Token);

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            cts.Cancel();
        }
    }
}
