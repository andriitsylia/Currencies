using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Extensions.Polling;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;

namespace TelegramBot
{
    public static class Program
    {
        private static TelegramBotClient botClient;
        public static async Task Main()
        {
            //            Worker();
            botClient = new TelegramBotClient("2142968090:AAGoUGYNrs7xMGt-n5apOkGD6Xw2128NRGE");

            User me = await botClient.GetMeAsync();
            Console.Title = me.Username ?? "My awesome telegram bot";

            using CancellationTokenSource cts = new CancellationTokenSource();

            ReceiverOptions receiverOption = new ReceiverOptions { AllowedUpdates = { } };

            botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOption, cts.Token);
            
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            cts.Cancel();

        }

        static void Worker()
        {
            TelegramBotClient botClient = new TelegramBotClient("2142968090:AAGoUGYNrs7xMGt-n5apOkGD6Xw2128NRGE");

            using CancellationTokenSource cts = new CancellationTokenSource();
            var receiverOption = new ReceiverOptions { AllowedUpdates = { } };

            botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOption, cancellationToken: cts.Token);
            //User me = await botClient.GetMeAsync();
            //Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            cts.Cancel();
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message)
            {
                return;
            }
            
            if (update.Message!.Type != MessageType.Text)
            {
                return;
            }

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}");

            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "You said:\n" + messageText,
                cancellationToken: cancellationToken);
        }

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            
            Console.WriteLine(ErrorMessage);
            
            return Task.CompletedTask;
        }
    }
}
