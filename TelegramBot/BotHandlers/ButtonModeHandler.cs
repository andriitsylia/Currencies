using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Models;
using TelegramBot.Services;
using TelegramBot.Settings;

namespace TelegramBot.BotHandlers
{
    public class ButtonModeHandler
    {

        public static Banks _banks;
        public static Bank currentBank;
        public static DateTime currentDate;
        public static bool IsDateSelected;
        public static string currentCurrency;
        public static PrivatBankRatesSourceModel ratesSource;
        public static CurrencyListServiceModel currencyList;

        public static async Task Handler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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
                    currentCurrency = string.Empty;

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
                    currentCurrency = string.Empty;

                    ratesSource = JsonRatesParse.Parse(currentBank, currentDate);
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
    }
}
