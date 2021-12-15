using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Models;
using TelegramBot.Settings;

namespace TelegramBot.Services
{
    public class Handlers
    {
        private static Banks _banks;
        static Bank currentBank;
        static DateTime currentDate;
        static string currentCurrency;
        static PrivatBankRatesSourceModel ratesSource;
        static CurrencyListServiceModel currencyList;

        public static async Task CallbackQueryHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Message sentMessage;
            Console.WriteLine("InlineMessageId is " + update.CallbackQuery.Data);
            string[] command = update.CallbackQuery.Data.Split(" ");

            switch (command[0])
            {
                case "/bank":
                    await botClient.AnswerCallbackQueryAsync(
                        update.CallbackQuery.Id,
                        text: command[1],
                        cancellationToken: cancellationToken);
                    
                    foreach (var bank in _banks.Items)
                    {
                        bank.Name += " 1";
                    }

                    sentMessage = await botClient.EditMessageReplyMarkupAsync(
                        chatId: update.CallbackQuery.Message.Chat.Id,
                        messageId: update.CallbackQuery.Message.MessageId,
                        replyMarkup: ReplyKeyboard.InlineBanksKeyboard(_banks),
                        cancellationToken: cancellationToken);

                    break;

                case "/date":
                    bool pressButton = true;
                    switch (command[1])
                    {
                        case "year-":
                            currentDate = currentDate.AddYears(-1);
                            break;
                        case "year":
                            pressButton = false;
                            break;
                        case "year+":
                            currentDate = currentDate.AddYears(1);
                            break;
                        case "month-":
                            currentDate = currentDate.AddMonths(-1);
                            break;
                        case "month":
                            pressButton = false;
                            break;
                        case "month+":
                            currentDate = currentDate.AddMonths(1);
                            break;
                        case "0":
                            pressButton = false;
                            break;
                        default:
                            pressButton = false;
                            break;
                    }
                    
                    await botClient.AnswerCallbackQueryAsync(
                        update.CallbackQuery.Id,
                        text: currentDate.ToString(),
                        cancellationToken: cancellationToken);

                    if (pressButton)
                    {

                        sentMessage = await botClient.EditMessageReplyMarkupAsync(
                            chatId: update.CallbackQuery.Message.Chat.Id,
                            messageId: update.CallbackQuery.Message.MessageId,
                            replyMarkup: ReplyKeyboard.InlineDateKeyboard(currentDate),
                            cancellationToken: cancellationToken);
                    }

                    break;

                case "/currency":
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: update.CallbackQuery.Message.Chat.Id,
                        text: "Your choice: " + command[1],
                        cancellationToken: cancellationToken);

                    await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id,
                        text: command[1],
                        cancellationToken: cancellationToken);
                    break;

                    

                case "/hide_currency_keyboard":
                    sentMessage = await botClient.EditMessageReplyMarkupAsync(
                        update.CallbackQuery.Message.Chat.Id,
                        update.CallbackQuery.Message.MessageId,
                        replyMarkup: null,
                        cancellationToken: cancellationToken);
                    break;
            }
        }

        public static async Task MessageHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Message sentMessage;
            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;
            string[] command = messageText.Split(" ");

            switch (command[0])
            {
                case "/start":
                    await Usage(botClient, update, cancellationToken);

                    currentBank = null;
                    currentDate = DateTime.MaxValue;
                    currentCurrency = string.Empty;
                    _banks = new BanksListFromSettings().Get();

                    StringBuilder message = new();
                    message.Append("*Please, choose the bank*:\n");
                    foreach (var bank in _banks.Items)
                    {
                        message.Append(bank.Name + "\n");
                    }
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: message.ToString(),
                        parseMode: ParseMode.MarkdownV2,
                        replyMarkup: null,
                        cancellationToken: cancellationToken);

                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: message.ToString(),
                        parseMode: ParseMode.MarkdownV2,
                        replyMarkup: ReplyKeyboard.InlineBanksKeyboard(_banks),
                        cancellationToken: cancellationToken);

                    break;

                case "/bank":
                    if (_banks == null)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Restart the bot",
                            replyMarkup: null,
                            cancellationToken: cancellationToken);

                        await Usage(botClient, update, cancellationToken);
                        break;
                    }

                    if (command.Length < 2)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Parameter _bank_ is missing",
                            parseMode: ParseMode.MarkdownV2,
                            replyToMessageId: update.Message.MessageId,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);

                        await Usage(botClient, update, cancellationToken);
                        break;
                    }

                    currentBank = _banks.Items.FirstOrDefault(b => b.Name.ToUpper() == command[1].ToUpper());

                    if (currentBank != null)
                    {
                        currentDate = DateTime.MaxValue;
                        currentCurrency = String.Empty;

                        sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "You choose the *" + currentBank.Name + "*\nEnter the date",
                        parseMode: ParseMode.MarkdownV2,
                        replyMarkup: null,
                        cancellationToken: cancellationToken);
                    }
                    else
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "I can't find the _" + command[1] + "_ bank",
                            parseMode: ParseMode.MarkdownV2,
                            replyToMessageId: update.Message.MessageId,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);
                    }

                    break;

                case "/date":
                    if (currentBank == null)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Choose the bank",
                            replyMarkup: null,
                            cancellationToken: cancellationToken);

                        await Usage(botClient, update, cancellationToken);
                        break;
                    }

                    if (command.Length < 2)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Parameter _date_ is missing",
                            parseMode: ParseMode.MarkdownV2,
                            replyToMessageId: update.Message.MessageId,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);

                        await Usage(botClient, update, cancellationToken);
                        break;
                    }

                    if (DateTime.TryParse(command[1], out currentDate))
                    {
                        currentCurrency = String.Empty;

                        sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Date is " + currentDate.ToString(currentBank.DateFormat),
                        replyMarkup: null,
                        cancellationToken: cancellationToken);

                        string jsonData = new BankCurrencyRates(currentBank).GetPerDateAsJson(currentDate).Result;
                        JsonElement root = JsonDocument.Parse(jsonData).RootElement;

                        ratesSource = JsonSerializer.Deserialize<PrivatBankRatesSourceModel>(root.ToString());
                        currencyList = new CurrencyListServiceModel(ratesSource);

                        sentMessage = await botClient.SendTextMessageAsync(
                           chatId: chatId,
                           text: "Please, choose any of the following currency :\n" + String.Join(" ", currencyList.Currencies),
                           replyMarkup: null,
                           cancellationToken: cancellationToken);

                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Date is " + currentDate.ToString(currentBank.DateFormat),
                            replyMarkup: ReplyKeyboard.InlineCurrencyKeyboard(currencyList),
                            cancellationToken: cancellationToken);

                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Date",
                            replyMarkup: ReplyKeyboard.InlineDateKeyboard(currentDate),
                            cancellationToken: cancellationToken);
                    }
                    else
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "The date isn't in _dd\\.mm\\.yyyy_ format",
                            parseMode: ParseMode.MarkdownV2,
                            replyToMessageId: update.Message.MessageId,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);
                    }

                    break;

                case "/currency":
                    if (currentDate == DateTime.MaxValue)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Enter the date",
                            replyMarkup: null,
                            cancellationToken: cancellationToken);

                        await Usage(botClient, update, cancellationToken);
                        break;
                    }

                    if (command.Length < 2)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Сurrency isn't specified",
                            replyToMessageId: update.Message.MessageId,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);

                        await Usage(botClient, update, cancellationToken);
                        break;
                    }

                    currentCurrency = currencyList?.Currencies.Find(c => !string.IsNullOrWhiteSpace(c) && c.ToUpper() == command[1].ToUpper());

                    if (!string.IsNullOrWhiteSpace(currentCurrency))
                    {
                        CurrencyRateServiceModel currencyRate = new(ratesSource, currentCurrency);
                        ReportModel rep = new(currencyRate);

                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: rep.Report,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);
                    }
                    else
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "I can't find the _" + command[1] + "_ currency",
                            parseMode: ParseMode.MarkdownV2,
                            replyToMessageId: update.Message.MessageId,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);
                    }
                    break;

                case "/help":

                default:
                    await Usage(botClient, update, cancellationToken);
                    break;
            }
            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}");
        }

        public static async Task Usage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var chatId = update.Message.Chat.Id;
            string message = "*Bot usage:*\n"
                           + "/start \\- restart the bot\n"
                           + "/bank _bank_\n"
                           + "/date _dd\\.mm\\.yyyy_\n"
                           + "/currency _currency_\n"
                           + "/help";

            Message sentMessage = await botClient.SendTextMessageAsync(
                       chatId: chatId,
                       text: message,
                       parseMode: ParseMode.MarkdownV2,
                       replyMarkup: null,
                       cancellationToken: cancellationToken);
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            switch (update.Type)
            {
                case UpdateType.CallbackQuery:
                    await CallbackQueryHandler(botClient, update, cancellationToken);
                    break;

                case UpdateType.Message:
                    await MessageHandler(botClient, update, cancellationToken);
                    break;

                default:
                    break;
            }
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
