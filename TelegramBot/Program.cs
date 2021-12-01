using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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

            if (update.Message.Type != MessageType.Text)
            {
                return;
            }

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}");

            //Message sentMessage = await botClient.SendTextMessageAsync(
            //    chatId: chatId,
            //    text: "*You said*:\n" + messageText,
            //    cancellationToken: cancellationToken);

            //Message sentMessage = await botClient.SendTextMessageAsync(
            //    chatId: chatId,
            //    text: "Hi\\! _My name_ is *Andrii*\\! Press [https://www\\.google\\.com]",
            //    parseMode: ParseMode.MarkdownV2,
            //    disableNotification: true,
            //    replyToMessageId: update.Message.MessageId,
            //    replyMarkup: new InlineKeyboardMarkup(
            //        InlineKeyboardButton.WithUrl(
            //            "Press button",
            //            "https://www.google.com")),
            //    cancellationToken: cancellationToken);

            //KeyboardButton[] row1 = { "10", "20", "30", "40", "50", "60", "70", "80" };
            //ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            //{
            //    row1,
            //    new KeyboardButton[] { "11", "22", "33", "44", "55", "66", "77", "88"},
            //})
            //{
            //    ResizeKeyboard = true
            //};

            //Message sentMessage = await botClient.SendTextMessageAsync(
            //    chatId: chatId,
            //    text: "Choose a response",
            //    replyMarkup: replyKeyboardMarkup,
            //    cancellationToken: cancellationToken);

            //ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            //{
            //    KeyboardButton.WithRequestLocation("Share Location"),
            //    KeyboardButton.WithRequestContact("Share Contact"),
            //});

            //Message sentMessage = await botClient.SendTextMessageAsync(
            //    chatId: chatId,
            //    text: "Who or where are you?",
            //    replyMarkup: replyKeyboardMarkup,
            //    cancellationToken: cancellationToken);

            //        InlineKeyboardMarkup inlineKeyboard = new(new[]
            //{
            //    // first row
            //    new []
            //    {
            //        InlineKeyboardButton.WithCallbackData(text: "1.1", callbackData: "11"),
            //        InlineKeyboardButton.WithCallbackData(text: "1.2", callbackData: "12"),
            //    },
            //    // second row
            //    new []
            //    {
            //        InlineKeyboardButton.WithCallbackData(text: "2.1", callbackData: "21"),
            //        InlineKeyboardButton.WithCallbackData(text: "2.2", callbackData: "22"),
            //    },
            //});

            //            InlineKeyboardMarkup inlineKeyboard = new(new[]
            //    {
            //        InlineKeyboardButton.WithUrl(
            //            text: "Link to the Repository",
            //            url: "https://github.com/TelegramBots/Telegram.Bot"
            //        )
            //    }
            //);

            InlineKeyboardMarkup inlineKeyboard = new(new[]
    {
        InlineKeyboardButton.WithSwitchInlineQuery("switch_inline_query"),
        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("switch_inline_query_current_chat"),
    }
);

            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Removing keyboard",
                replyMarkup: inlineKeyboard,
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
