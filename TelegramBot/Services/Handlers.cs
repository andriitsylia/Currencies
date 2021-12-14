using System;
using System.Collections.Generic;
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
        static Bank currentBank;
        static DateTime currentDate;
        static string currentCurrency;
        static PrivatBankRatesSourceModel currencyRatesSource;
        static PrivatBankCurrencyListServiceModel currencyList;

        public static async Task CallbackQueryHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Message sentMessage;
            Console.WriteLine("InlineMessageId is " + update.CallbackQuery.Data);
            string[] command = update.CallbackQuery.Data.Split(" ");

            switch (command[0])
            {
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

                    currentBank = new();
                    currentDate = new();
                    currentCurrency = String.Empty;

                    StringBuilder message = new();
                    message.Append("*Please, choose the bank*:\n");
                    //foreach (var bank in banks)
                    //{
                    //    message.Append(bank.Name + "\r\n");
                    //}
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: message.ToString(),
                        parseMode: ParseMode.MarkdownV2,
                        replyMarkup: null,
                        cancellationToken: cancellationToken);
                    break;

                case "/bank":
                    if ((command.Length < 2))
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
                    //currentBank = banks.Find(b => b.Name.ToUpper() == command[1].ToUpper());
                    if (currentBank != null)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "You choose the *" + currentBank.Name + "*\n\nEnter the date",
                        parseMode: ParseMode.MarkdownV2,
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
                    if ((command.Length < 2))
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
                        sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: currentDate.ToString("dd.MM.yyyy"),
                        cancellationToken: cancellationToken);

                        GetJsonDataFromBank bankRates = new(currentBank);
                        string bankJsonData = bankRates.Get(currentDate).Result;
                        JsonDocument doc = JsonDocument.Parse(bankJsonData);
                        JsonElement root = doc.RootElement;
                        //foreach (var bank in banks)
                        //{
                        //    if (bank.Name == currentBank.Name)
                        //    {
                        //        currencyRatesSource = JsonSerializer.Deserialize<PrivatBankRatesSourceModel>(root.ToString());
                        //        currencyList = new(currencyRatesSource);
                        //        break;
                        //    }
                        //}
                        sentMessage = await botClient.SendTextMessageAsync(
                           chatId: chatId,
                           text: "Please, choose the any of the following currency :\n" + String.Join(" ", currencyList.Currencies),
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

                    //if (currencyList != null)
                    //{
                        currentCurrency = currencyList?.Currencies.Find(c => !string.IsNullOrWhiteSpace(c) && c.ToUpper() == command[1].ToUpper());
                    //}

                    if (!string.IsNullOrWhiteSpace(currentCurrency))
                    {
                        Currency c = (Currency)Enum.Parse(typeof(Currency), currentCurrency.ToUpper());
                        PrivatBankCurrencyRateServiceModel privatBankCurrencyRate =
                            new PrivatBankCurrencyRateServiceModel(currencyRatesSource, c);
                        PrivatBankReportModel rep = new PrivatBankReportModel(privatBankCurrencyRate);

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

                case "Hide main keyboard":
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Hide main keyboard",
                        replyMarkup: new ReplyKeyboardRemove(),
                        cancellationToken: cancellationToken);
                    break;

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
                           + "/currency _currency_";

            Message sentMessage = await botClient.SendTextMessageAsync(
                       chatId: chatId,
                       text: message,
                       parseMode: ParseMode.MarkdownV2,
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
