using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Models;
using TelegramBot.Services;
using TelegramBot.Settings;

namespace TelegramBot
{
    public class Program
    {
        private static TelegramBotClient botClient;
        
        static IConfiguration mainSettings;
        static string token;
        static List<Bank> banks;

        public static async Task Main()
        {
            mainSettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            token = mainSettings.GetSection("TelegramToken").Value;
            banks = mainSettings.GetSection("Bank").Get<List<Bank>>();

            botClient = new TelegramBotClient(token);
            using CancellationTokenSource cts = new CancellationTokenSource();

            await Worker(botClient, cts);

            Console.ReadLine();
            cts.Cancel();
        }

        public static async Task Worker(ITelegramBotClient botClient, CancellationTokenSource cts)
        {
            User me = await botClient.GetMeAsync();
            Console.Title = me.Username ?? "My awesome telegram bot";

            ReceiverOptions receiverOption = new ReceiverOptions { AllowedUpdates = { } };

            botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOption, cts.Token);
            Console.WriteLine($"Start listening for @{me.Username}");
        }

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
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Choose the bank",
                        replyMarkup: ReplyKeyboard.ReplyMainKeyboard(),
                        cancellationToken: cancellationToken);
                    break;

                case "/bank":
                    StringBuilder message = new();
                    message.Append("*Please, make a choice*:\r\n");
                    foreach (var bank in banks)
                    {
                        message.Append(bank.Name + "\r\n");
                    }
                    message.Append("\r\n*Usage*: /bank \\<_Bank_\\>");
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: message.ToString(),
                        parseMode: ParseMode.MarkdownV2,
                        //replyMarkup: replyKeyboardMarkup,
                        cancellationToken: cancellationToken);
                    break;

                case "Currency":
                case "Banks":
                    sentMessage = await botClient.SendTextMessageAsync(
                           chatId: chatId,
                           text: "Parameters is not enough:\r\n/banks date currency",
                           replyToMessageId: update.Message.MessageId,
                           replyMarkup: null,
                           cancellationToken: cancellationToken);
                    break;

                case "/banks":
                    if (command.Length < 3)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                           chatId: chatId,
                           text: "Parameters is not enough:\r\n/banks date currency",
                           replyToMessageId: update.Message.MessageId,
                           replyMarkup: null,
                           cancellationToken: cancellationToken);
                    }
                    else
                    {
                        DateTime d;
                        if (DateTime.TryParse(command[1], out d))
                        {
                            GetJsonDataFromBank privatBankRates = new(BankSettings.PrivatBank, "https://api.privatbank.ua/p24api/exchange_rates?json&date=");
                            string privatBankJsonData = privatBankRates.Get(d).Result;
                            JsonDocument doc = JsonDocument.Parse(privatBankJsonData);
                            JsonElement root = doc.RootElement;
                            PrivatBankRatesSourceModel currencyRatesSource = JsonSerializer.Deserialize<PrivatBankRatesSourceModel>(root.ToString());
                            PrivatBankCurrencyListServiceModel privatBankCurrencyList = new(currencyRatesSource);

                            int pos = privatBankCurrencyList.Currencies.IndexOf(command[2].ToUpper());

                            if (pos != -1)
                            {
                                sentMessage = await botClient.SendTextMessageAsync(
                                   chatId: chatId,
                                   text: "Please, choose the currency:",
                                   replyMarkup: ReplyKeyboard.InlineCurrencyKeyboard(privatBankCurrencyList),
                                   cancellationToken: cancellationToken);

                                Currency c = (Currency)Enum.Parse(typeof(Currency), command[2].ToUpper());
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
                                    text: "Currency isn't valid or isn't in currency list",
                                    replyMarkup: null,
                                    cancellationToken: cancellationToken);
                            }
                        }
                        else
                        {
                            sentMessage = await botClient.SendTextMessageAsync(
                           chatId: chatId,
                           text: "Date isn't valid:\r\ndate format is dd.mm.yyyy",
                           replyToMessageId: update.Message.MessageId,
                           replyMarkup: null,
                           cancellationToken: cancellationToken);
                        }
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
                    sentMessage = await botClient.SendTextMessageAsync(
                       chatId: chatId,
                       text: "Hi\\! _My name_ is *Andrii*\\! Press [https://www\\.google\\.com]",
                       parseMode: ParseMode.MarkdownV2,
                       disableNotification: false,
                       replyToMessageId: update.Message.MessageId,
                       replyMarkup: null,
                       cancellationToken: cancellationToken);
                    break;
            }
            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}");
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
