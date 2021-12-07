using System;
using System.Collections.Generic;
using System.Net.Http;
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

namespace TelegramBot
{
    public static class Program
    {
        private static TelegramBotClient botClient;

        public static async Task Main()
        {
            botClient = new TelegramBotClient("2142968090:AAGoUGYNrs7xMGt-n5apOkGD6Xw2128NRGE");


            //GetExchangeRateFromBank ratePrivatBank = new(Bank.PrivatBank.ToString());
            //Task<string> jsonPrivatBankRate = ratePrivatBank.GetPerDateAsJson(new DateTime(2021, 12, 03));
            //Task<string> jsonPrivatBankRate = ratePrivatBank.GetCashAsJson();

            //Console.WriteLine(jsonPrivatBankRate.Result);
            //Console.WriteLine("----------");

            //using JsonDocument doc = JsonDocument.Parse(jsonPrivatBankRate.Result);
            //JsonElement root = doc.RootElement;
            //Console.WriteLine(root);
            //PBExchangeRatePerDateList privatBankExchangeRatePerDate;
            //privatBankExchangeRatePerDate = JsonSerializer.Deserialize<PBExchangeRatePerDateList>(root.ToString());
            //PBExchangeRateList pBExchangeRateList = new();
            //pBExchangeRateList.Rates = new();

            //for (int i = 0; i < root.GetArrayLength(); i++)
            //{
            //    Console.WriteLine(root[i].ToString());
            //    PBExchangeRate rate = new();
            //    rate = JsonSerializer.Deserialize<PBExchangeRate>(root[i].ToString());
            //    pBExchangeRateList.Rates.Add(rate);
            //    //Console.WriteLine(rate.ccy);
            //    //Console.WriteLine(rate.base_ccy);
            //    //Console.WriteLine(rate.buy);
            //    //Console.WriteLine(rate.sale);
            //    //Console.WriteLine("*****");
            //}
            //PBExchangeRate rate1 = PBExchangeRateNow.GetFor("BTC", pBExchangeRateList);

            //Console.WriteLine(rate1.ccy);
            //Console.WriteLine(rate1.base_ccy);
            //Console.WriteLine(rate1.buy);
            //Console.WriteLine(rate1.sale);
            //Console.WriteLine("*****");
            //Console.WriteLine("----------");

            //JsonElement er = root.GetProperty("exchangeRate");
            //Console.WriteLine(er);
            //Console.WriteLine("----------");

            //using JsonDocument doc2 = JsonDocument.Parse(er.ToString());
            //JsonElement root2 = doc2.RootElement;

            //foreach (var r in root2.EnumerateArray())
            //{
            //    Console.WriteLine(r);
            //    foreach (var item in r.EnumerateObject())
            //    {
            //        Console.WriteLine($"{item.Name}: {item.Value}");
            //    }
            //}
            //Console.WriteLine(root2.GetArrayLength());
            

            User me = await botClient.GetMeAsync();
            Console.Title = me.Username ?? "My awesome telegram bot";

            using CancellationTokenSource cts = new CancellationTokenSource();

            ReceiverOptions receiverOption = new ReceiverOptions { AllowedUpdates = { } };

            botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOption, cts.Token);

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            cts.Cancel();
        }

        public static async Task CallbackQueryHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Message sentMessage;
            Console.WriteLine("InlineMessageId is " + update.CallbackQuery.Data);
            switch (update.CallbackQuery.Data)
            {
                case "11":
                //sentMessage = await botClient.SendTextMessageAsync(
                //    chatId: update.Message.Chat.Id,
                //    text: update.Message.Text,
                //    replyMarkup: inlineKeyboard,
                //    cancellationToken: cancellationToken);
                //break;

                case "USD":
                    sentMessage = await botClient.EditMessageReplyMarkupAsync(
                        update.CallbackQuery.Message.Chat.Id,
                        update.CallbackQuery.Message.MessageId,
                        replyMarkup: null,
                        cancellationToken: cancellationToken);
                    break;
            }
        }

        public static ReplyKeyboardMarkup ReplyCurrencyKeyboard(PrivatBankCurrencyListServiceModel privatBankCurrencyList)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup;
            List<KeyboardButton> row = new();
            List<List<KeyboardButton>> rows = new();
            int i = 0;
            foreach (var currency in privatBankCurrencyList.Currencies)
            {
                if (!string.IsNullOrWhiteSpace(currency))
                {
                    i++;
                    if (i <= 10)
                    {
                        row.Add(currency);
                    }
                    else
                    {
                        rows.Add(row);
                        i = 1;
                        row = new List<KeyboardButton>();
                        row.Add(currency);
                    }
                }
            }
            rows.Add(row);

            replyKeyboardMarkup = new(rows) { ResizeKeyboard = true };
             return replyKeyboardMarkup;
        }

        public static InlineKeyboardMarkup InlineCurrencyKeyboard(PrivatBankCurrencyListServiceModel privatBankCurrencyList)
        {
            InlineKeyboardMarkup inlineKeyboardMarkup;
            List<InlineKeyboardButton> row = new();
            List<List<InlineKeyboardButton>> rows = new();
            int i = 0;
            foreach (var currency in privatBankCurrencyList.Currencies)
            {
                if (!string.IsNullOrWhiteSpace(currency))
                {
                    i++;
                    if (i <= 6)
                    {
                        row.Add(InlineKeyboardButton.WithCallbackData(text: currency, callbackData: currency));
                    }
                    else
                    {
                        rows.Add(row);
                        i = 1;
                        row = new List<InlineKeyboardButton>();
                        row.Add(currency);
                    }
                }
            }
            rows.Add(row);

            inlineKeyboardMarkup = new(rows);
            return inlineKeyboardMarkup;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine("Update type is " + update.Type.ToString());
            Message sentMessage;

            switch (update.Type)
            {
                case UpdateType.CallbackQuery:
                    await CallbackQueryHandler(botClient, update, cancellationToken);
                    break;

                case UpdateType.Message:
                    var chatId = update.Message.Chat.Id;
                    var messageText = update.Message.Text;
                    ReplyKeyboardMarkup replyKeyboardMarkup;
                    InlineKeyboardMarkup inlineKeyboard = new(new[]
                                {
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData(text: "1.1", callbackData: "11"),
                                InlineKeyboardButton.WithCallbackData(text: "1.2", callbackData: "12"),
                            },
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData(text: "2.1", callbackData: "21"),
                                InlineKeyboardButton.WithCallbackData(text: "2.2", callbackData: "22"),
                            },
                        });


                    switch (messageText)
                    {
                        case "/start":
                        case "/bank":

                            sentMessage = await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Choose the bank",
                                //replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                            break;

                        case "/privatbank":
                        case "PrivatBank":

                            GetJsonDataFromBank privatBankRates = new(Bank.PrivatBank, "https://api.privatbank.ua/p24api/exchange_rates?json&date=");
                            string privatBankJsonData = privatBankRates.Get(DateTime.Now).Result;
                            JsonDocument doc = JsonDocument.Parse(privatBankJsonData);
                            JsonElement root = doc.RootElement;
                            PrivatBankCurrencyRatesSourceModel currencyRatesSource = JsonSerializer.Deserialize<PrivatBankCurrencyRatesSourceModel>(root.ToString());
                            PrivatBankCurrencyListServiceModel privatBankCurrencyList = new(currencyRatesSource);

                            //replyKeyboardMarkup = new(new[]
                            //{
                            //    new KeyboardButton[] {"PrivatBank"}
                            //})
                            //{
                            //    ResizeKeyboard = true
                            //};

                            //sentMessage = await botClient.SendTextMessageAsync(
                            //    chatId: chatId,
                            //    text: string.Join(" ", privatBankCurrencyList.Currencies),
                            //    replyMarkup: ReplyCurrencyKeyboard(privatBankCurrencyList),
                            //    cancellationToken: cancellationToken);

                            sentMessage = await botClient.SendTextMessageAsync(
                               chatId: chatId,
                               text: "Please, choose the currency:",
                               replyMarkup: InlineCurrencyKeyboard(privatBankCurrencyList),
                               cancellationToken: cancellationToken);
                            break;

                        case "/inline":

                            sentMessage = await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: messageText,
                                replyMarkup: inlineKeyboard,
                                cancellationToken: cancellationToken);
                            break;

                        case "/removekeyboard":
                            sentMessage = await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: messageText,
                                replyMarkup: new ReplyKeyboardRemove(),
                                cancellationToken: cancellationToken);
                            break;

                        default:
                            break;
                    }
                    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}");

                    break;

                default:
                    break;
            }


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
            //    text: messageText,
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
    //            // first row
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

            //        InlineKeyboardMarkup inlineKeyboard = new(new[]
            //{
            //    InlineKeyboardButton.WithSwitchInlineQuery("switch_inline_query"),
            //    InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("switch_inline_query_current_chat"),
            //}
            //);

            //Message sentMessage = await botClient.SendTextMessageAsync(
            //    chatId: chatId,
            //    text: messageText,
            //    replyMarkup: inlineKeyboard,
            //    cancellationToken: cancellationToken);



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
