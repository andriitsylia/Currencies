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

namespace TelegramBot.Handlers
{
    public class CallbackQueryHandler
    {

        public static Banks _banks;
        public static Bank currentBank;
        public static DateTime currentDate;
        public static bool IsDateSelected;
        public static string currentCurrency;
        public static PrivatBankRatesSourceModel ratesSource;
        public static CurrencyListServiceModel currencyList;

        public static async Task Handler(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var message = callbackQuery.Message;

            Console.WriteLine("InlineMessageId is " + callbackQuery.Data);

            string[] command = callbackQuery.Data.Split(" ");

            switch (command[0])
            {
                case BotCommands.CMD_BANK:
                    await BotMessage.SendAnswerCallbackQuery(botClient, callbackQuery.Id);
                    await CommandBankHandler.Handler(botClient, message, callbackQuery.Data);
                    break;

                case BotCommands.CMD_DATE:
                    //bool isDayButtonPressed = true;
                    //if (int.TryParse(command[1], out int buttonValue))
                    //{
                    //    if (buttonValue is >= 1 and <= 31)
                    //    {
                    //        currentDate = new DateTime(currentDate.Year, currentDate.Month, buttonValue);
                    //    }
                    //    isDayButtonPressed = false;
                    //}
                    //else
                    //{
                    //    switch (command[1])
                    //    {
                    //        case BotCommands.PARAM_YEAR_DECREMENT:
                    //            currentDate = currentDate.AddYears(-1);
                    //            break;
                    //        case BotCommands.PARAM_YEAR_INCREMENT:
                    //            currentDate = currentDate.AddYears(1);
                    //            break;
                    //        case BotCommands.PARAM_MONTH_DECREMENT:
                    //            currentDate = currentDate.AddMonths(-1);
                    //            break;
                    //        case BotCommands.PARAM_MONTH_INCREMENT:
                    //            currentDate = currentDate.AddMonths(1);
                    //            break;
                    //        default:
                    //            isDayButtonPressed = false;
                    //            break;
                    //    }
                    //}

                    await BotMessage.SendAnswerCallbackQuery(botClient, callbackQuery.Id);//,
                        //MessageHandler.currentDate.ToString(MessageHandler.currentBank.DateFormat));
                    
                    await CommandDateHandler.Handler(botClient, message, callbackQuery.Data);

                    //if (isDayButtonPressed)
                    //{
                    //    await BotMessage.EditMessage(
                    //        botClient,
                    //        callbackQuery.Message.Chat.Id,
                    //        callbackQuery.Message.MessageId,
                    //        ReplyKeyboard.InlineDateKeyboard(currentDate));
                    //}
                    break;

                case BotCommands.CMD_DATECONFIRM:
                    await BotMessage.SendAnswerCallbackQuery(botClient, callbackQuery.Id);
                    currentCurrency = string.Empty;

                    MessageHandler.ratesSource = JsonRatesParse.Parse(MessageHandler.currentBank, MessageHandler.currentDate);
                    MessageHandler.currencyList = new CurrencyListServiceModel(MessageHandler.ratesSource);

                    if (MessageHandler.currencyList.Currencies.Count == 0)
                    {
                        await BotMessage.SendMessage(botClient, chatId, $"No currency rates on {MessageHandler.currentDate.ToString(MessageHandler.currentBank.DateFormat)}");
                        break;
                    }
                    await BotMessage.SendMessage(botClient, chatId, $"Select {MessageHandler.currentDate.ToString(MessageHandler.currentBank.DateFormat)}");

                    IsDateSelected = true;

                    await BotMessage.SendMessageMarkdown(botClient, chatId, "Press *Currency* button to select the currency");
                    break;

                case BotCommands.CMD_CURRENCY:
                    await BotMessage.SendAnswerCallbackQuery(botClient, callbackQuery.Id, command[1]);

                    currentCurrency = currencyList?.Currencies.Find(c => !string.IsNullOrWhiteSpace(c) && c.ToUpper() == command[1].ToUpper());

                    CurrencyRateServiceModel currencyRate = new(ratesSource, currentCurrency);
                    ReportModel rep = new(currencyRate);

                    await BotMessage.SendMessage(botClient, chatId, rep.Report);
                    break;
            }
        }
    }
}
