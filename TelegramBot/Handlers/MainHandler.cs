using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Models;
using TelegramBot.Services;

namespace TelegramBot.Handlers
{
    public class MainHandler
    {
        static CurrentSession currentSession;

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var handler = update.Type switch
                {
                    UpdateType.Message => MessageHandler(botClient, update.Message),
                    UpdateType.EditedMessage => MessageHandler(botClient, update.EditedMessage),
                    UpdateType.CallbackQuery => CallbackQueryHandler(botClient, update.CallbackQuery),
                    _ => UnknownUpdateHandlerAsync(botClient, update)
                };
                await handler;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(botClient, ex, cancellationToken);
            }
        }

        public static async Task MessageHandler(ITelegramBotClient botClient, Message message)
        {
            var chatId = message.Chat.Id;
            var messageText = message.Text;
            string[] command = messageText.Split(" ");
            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}");

            switch (command[0])
            {
                case Constants.BotCommand.CMD_START:
                    await CommandHelpHandler.Handler(botClient, message);
                    await CommandStartHandler.Handler(botClient, message);
                    currentSession = new CurrentSession();
                    break;

                case Constants.BotCommand.CMD_BANK:
                    await CommandBankHandler.Handler(botClient, message, messageText, currentSession);
                    break;

                case Constants.BotCommand.CMD_DATE:
                    await CommandDateHandler.Handler(botClient, message, messageText, currentSession);
                    break;

                case Constants.BotCommand.CMD_CURRENCY:
                    await CommandCurrencyHandler.Handler(botClient, message, messageText, currentSession);
                    break;

                case Constants.BotCommand.BUTTON_BANK:
                    await CommandBankHandler.Handler(botClient, message, messageText, currentSession);
                    break;

                case Constants.BotCommand.BUTTON_DATE:
                    await CommandDateHandler.Handler(botClient, message, messageText, currentSession);
                    break;

                case Constants.BotCommand.BUTTON_CURRENCY:
                    await CommandCurrencyHandler.Handler(botClient, message, messageText, currentSession);
                    break;

                case Constants.BotCommand.CMD_HELP:
                default:
                    await CommandHelpHandler.Handler(botClient, message);
                    break;
            }
        }

        public static async Task CallbackQueryHandler(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var message = callbackQuery.Message;
            string[] command = callbackQuery.Data.Split(" ");
            Console.WriteLine($"Received a '{callbackQuery.Data}' inlinemessage in chat {chatId}");

            switch (command[0])
            {
                case Constants.BotCommand.CMD_BANK:
                    await BotMessage.SendAnswerCallbackQuery(botClient, callbackQuery.Id);
                    await CommandBankHandler.Handler(botClient, message, callbackQuery.Data, currentSession);
                    break;

                case Constants.BotCommand.CMD_DATE:
                    await BotMessage.SendAnswerCallbackQuery(botClient, callbackQuery.Id);
                    await CommandDateHandler.Handler(botClient, message, callbackQuery.Data, currentSession);
                    break;

                case Constants.BotCommand.CMD_DATECONFIRM:
                    await BotMessage.SendAnswerCallbackQuery(botClient, callbackQuery.Id);
                    await CommandDateConfirmHandler.Handler(botClient, message, currentSession);
                    break;

                case Constants.BotCommand.CMD_CURRENCY:
                    await BotMessage.SendAnswerCallbackQuery(botClient, callbackQuery.Id, command[1]);
                    await CommandCurrencyHandler.Handler(botClient, message, callbackQuery.Data, currentSession);
                    break;
            }
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
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
