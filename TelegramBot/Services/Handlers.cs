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
        private const string BOT_MODE_TEXT = "Text";
        private const string BOT_MODE_BUTTON = "Button";

        private static Banks _banks;
        private static Bank currentBank;
        private static DateTime currentDate;
        private static bool IsDateSelected;
        private static string currentCurrency;
        private static PrivatBankRatesSourceModel ratesSource;
        private static CurrencyListServiceModel currencyList;

        public static async Task CallbackQueryHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Message sentMessage;
            var chatId = update.CallbackQuery.Message.Chat.Id;

            Console.WriteLine("InlineMessageId is " + update.CallbackQuery.Data);
            
            string[] command = update.CallbackQuery.Data.Split(" ");

            switch (command[0])
            {
                case "/bank":

                    await botClient.AnswerCallbackQueryAsync(
                        update.CallbackQuery.Id,
                        text: command[1],
                        cancellationToken: cancellationToken);

                    currentBank = _banks.Items.FirstOrDefault(b => b.Name.ToUpper() == command[1].ToUpper());

                    currentDate = DateTime.Today;
                    currentCurrency = String.Empty;

                    sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "*" + currentBank.Name + "*\n"
                        + "Press *Date* button to select the date",
                    parseMode: ParseMode.MarkdownV2,
                    replyMarkup: null,
                    cancellationToken: cancellationToken);
                    break;

                case "/date":
                    bool isDayButtonPressed = true;
                    if (int.TryParse(command[1], out int buttonValue))
                    {
                        if (buttonValue is >= 1 and <= 31)
                        {
                            currentDate = new DateTime(currentDate.Year, currentDate.Month, buttonValue);
                        }
                        isDayButtonPressed = false;
                    }
                    else
                    {
                        switch (command[1])
                        {
                            case "year-":
                                currentDate = currentDate.AddYears(-1);
                                break;
                            case "year":
                                isDayButtonPressed = false;
                                break;
                            case "year+":
                                currentDate = currentDate.AddYears(1);
                                break;
                            case "month-":
                                currentDate = currentDate.AddMonths(-1);
                                break;
                            case "month":
                                isDayButtonPressed = false;
                                break;
                            case "month+":
                                currentDate = currentDate.AddMonths(1);
                                break;
                            default:
                                isDayButtonPressed = false;
                                break;
                        }
                    }

                    await botClient.AnswerCallbackQueryAsync(
                        update.CallbackQuery.Id,
                        text: currentDate.ToString("dd.MM.yyyy"),
                        cancellationToken: cancellationToken);

                    if (isDayButtonPressed)
                    {

                        sentMessage = await botClient.EditMessageReplyMarkupAsync(
                            chatId: update.CallbackQuery.Message.Chat.Id,
                            messageId: update.CallbackQuery.Message.MessageId,
                            replyMarkup: ReplyKeyboard.InlineDateKeyboard(currentDate),
                            cancellationToken: cancellationToken);
                    }

                    break;

                case "/confirmdate":
                    currentCurrency = String.Empty;

                    string jsonData = new BankCurrencyRates(currentBank).GetPerDateAsJson(currentDate).Result;
                    JsonElement root = JsonDocument.Parse(jsonData).RootElement;

                    ratesSource = JsonSerializer.Deserialize<PrivatBankRatesSourceModel>(root.ToString());
                    currencyList = new CurrencyListServiceModel(ratesSource);

                    if (currencyList.Currencies.Count == 0)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                               chatId: chatId,
                               text: "The " + currentBank.Name
                                   + " hasn't information about currency rates at "
                                   + currentDate.ToString(currentBank.DateFormat) + "\n"
                                   + "Select another date",
                               cancellationToken: cancellationToken);
                        break;
                    }

                    sentMessage = await botClient.SendTextMessageAsync(
                       chatId: chatId,
                       text: "Select " + currentDate.ToString(currentBank.DateFormat),
                       cancellationToken: cancellationToken);

                    IsDateSelected = true;

                    sentMessage = await botClient.SendTextMessageAsync(
                       chatId: chatId,
                       text: "Press *Currency* button to select the currency",
                       parseMode: ParseMode.MarkdownV2,
                       cancellationToken: cancellationToken);
                    break;

                case "/currency":
                    await botClient.AnswerCallbackQueryAsync(
                        callbackQueryId: update.CallbackQuery.Id,
                        text: command[1],
                        cancellationToken: cancellationToken);

                    currentCurrency = currencyList?.Currencies.Find(c => !string.IsNullOrWhiteSpace(c) && c.ToUpper() == command[1].ToUpper());

                    CurrencyRateServiceModel currencyRate = new(ratesSource, currentCurrency);
                    ReportModel rep = new(currencyRate);

                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: rep.Report,
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
                case "/mode":
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Please, select a bot mode:",
                        replyMarkup: ReplyKeyboard.ReplyModeKeyboard(),
                        cancellationToken: cancellationToken);
                    break;

                case BOT_MODE_TEXT:
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Bot mode: " + BOT_MODE_TEXT,
                        replyMarkup: new ReplyKeyboardRemove(),
                        cancellationToken: cancellationToken);

                    await Usage(botClient, update, cancellationToken);

                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Type */start*",
                        parseMode: ParseMode.MarkdownV2,
                        cancellationToken: cancellationToken);

                    break;

                case BOT_MODE_BUTTON:
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Bot mode: " + BOT_MODE_BUTTON,
                        replyMarkup: new ReplyKeyboardRemove(),
                        cancellationToken: cancellationToken);

                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Please, select the _Bank_, the_Date_ and the _Currency_",
                        parseMode: ParseMode.MarkdownV2,
                        replyMarkup: ReplyKeyboard.ReplyMainKeyboard(),
                        cancellationToken: cancellationToken);

                    _banks = null;
                    break;

                case "/start":
                    currentBank = null;
                    currentDate = DateTime.Today;
                    IsDateSelected = false;
                    currentCurrency = string.Empty;
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Please, type */bankslist* to output the banks list",
                        parseMode: ParseMode.MarkdownV2,
                        replyMarkup: null,
                        cancellationToken: cancellationToken);
                    break;

                case "/bankslist":
                    _banks = new BanksListFromSettings().Get();

                    StringBuilder message = new();
                    message.Append("*Please, select the bank*:\n");

                    foreach (var bank in _banks.Items)
                    {
                        message.Append(bank.Name + "\n");
                    }
                    message.Append("\nType */bank* _bank_");

                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: message.ToString(),
                        parseMode: ParseMode.MarkdownV2,
                        replyMarkup: null,
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
                        currentDate = DateTime.Today;
                        currentCurrency = String.Empty;

                        sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "*" + currentBank.Name + "*\n"
                            + "\nType */date* _dd\\.mm\\.yyyy_",
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
                            text: "Select the bank",
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

                        string jsonData = new BankCurrencyRates(currentBank).GetPerDateAsJson(currentDate).Result;
                        JsonElement root = JsonDocument.Parse(jsonData).RootElement;
                        
                        ratesSource = JsonSerializer.Deserialize<PrivatBankRatesSourceModel>(root.ToString());
                        currencyList = new CurrencyListServiceModel(ratesSource);

                        if (currencyList.Currencies.Count == 0)
                        {
                            sentMessage = await botClient.SendTextMessageAsync(
                               chatId: chatId,
                               text: "The " + currentBank.Name
                                   + " hasn't information about currency rates at "
                                   + currentDate.ToString(currentBank.DateFormat) + "\n"
                                   + "Select another date",
                               cancellationToken: cancellationToken);
                            break;
                        }

                        IsDateSelected = true;

                        sentMessage = await botClient.SendTextMessageAsync(
                           chatId: chatId,
                           text: "Please, select any of the following currency :\n"
                                + String.Join(" ", currencyList.Currencies)
                                + "\n\nType */currency* _currency_",
                           parseMode: ParseMode.MarkdownV2,
                           replyMarkup: null,
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
                    if (!IsDateSelected)
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

                case "Bank":
                    IsDateSelected = false;
                    currentCurrency = string.Empty;

                    _banks = new BanksListFromSettings().Get();

                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Select the bank",
                        replyMarkup: ReplyKeyboard.InlineBanksKeyboard(_banks),
                        cancellationToken: cancellationToken);

                    break;

                case "Date":
                    if (currentBank == null)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Bank isn't selected",
                            cancellationToken: cancellationToken);
                        break;
                    }
                    
                    currentDate = DateTime.Today;
                    currentCurrency = string.Empty;
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Select the date, then press *Confirm date* button",
                        parseMode: ParseMode.MarkdownV2,
                        replyMarkup: ReplyKeyboard.InlineDateKeyboard(currentDate),
                        cancellationToken: cancellationToken);
                    break;

                case "Currency":
                    if (!IsDateSelected)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Date isn't selected",
                            cancellationToken: cancellationToken);
                        break;
                    }

                    currentCurrency = string.Empty;
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Select the currency",
                        replyMarkup: ReplyKeyboard.InlineCurrencyKeyboard(currencyList),
                        cancellationToken: cancellationToken);
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
                           + "/start \\- begin\\/restart work with the bot\n"
                           + "/bankslist\n"
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
