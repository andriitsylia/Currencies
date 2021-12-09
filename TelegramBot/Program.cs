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
            string[] commands = update.CallbackQuery.Data.Split(" ");
            switch (commands[0])
            {
                case "11":
                break;

                case "/currency":
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: update.CallbackQuery.Message.Chat.Id,
                        text: "You've entered " + commands[1],
                        cancellationToken: cancellationToken);
                    await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id,
                        text: "You've entered " + commands[1],
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
                    string[] commands = messageText.Split(" ");
                    switch (commands[0])
                    {
                        case "/start":
                            sentMessage = await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Choose the bank, currency and enter the date",
                                replyMarkup: ReplyKeyboard.ReplyMainKeyboard(),
                                cancellationToken: cancellationToken);
                            break;

                        case "/bank":
                            sentMessage = await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Choose the bank",
                                //replyMarkup: replyKeyboardMarkup,
                                cancellationToken: cancellationToken);
                            break;

                        case "Currency":
                        case "/banks":
                        case "Banks":
                            if (commands.Length < 3)
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
                                if (DateTime.TryParse(commands[1], out d))
                                {
                                    GetJsonDataFromBank privatBankRates = new(Bank.PrivatBank, "https://api.privatbank.ua/p24api/exchange_rates?json&date=");
                                    string privatBankJsonData = privatBankRates.Get(d).Result;
                                    JsonDocument doc = JsonDocument.Parse(privatBankJsonData);
                                    JsonElement root = doc.RootElement;
                                    PrivatBankCurrencyRatesSourceModel currencyRatesSource = JsonSerializer.Deserialize<PrivatBankCurrencyRatesSourceModel>(root.ToString());
                                    PrivatBankCurrencyListServiceModel privatBankCurrencyList = new(currencyRatesSource);
                                    
                                    int pos = privatBankCurrencyList.Currencies.IndexOf(commands[2].ToUpper());

                                    if (pos != -1)
                                    {
                                        sentMessage = await botClient.SendTextMessageAsync(
                                           chatId: chatId,
                                           text: "Please, choose the currency:",
                                           replyMarkup: ReplyKeyboard.InlineCurrencyKeyboard(privatBankCurrencyList),
                                           cancellationToken: cancellationToken);

                                        Currency c = (Currency)Enum.Parse(typeof(Currency), commands[2].ToUpper());
                                        PrivatBankCurrencyRateServiceModel privatBankCurrencyRate =
                                            new PrivatBankCurrencyRateServiceModel(currencyRatesSource, c);
                                        PrivatBankCurrencyRateReportModel rep = new PrivatBankCurrencyRateReportModel(privatBankCurrencyRate);
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

                            }

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

                    break;

                default:
                    
                    break;
            }


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
