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

        public static async Task Handler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var chatId = update.CallbackQuery.Message.Chat.Id;

            Console.WriteLine("InlineMessageId is " + update.CallbackQuery.Data);

            string[] command = update.CallbackQuery.Data.Split(" ");

            switch (command[0])
            {
                case BotCommands.CMD_BANK:

                    await BotMessage.SendAnswerCallbackQuery(botClient, update.CallbackQuery.Id, command[1]);

                    currentBank = _banks.Items.FirstOrDefault(b => b.Name.ToUpper() == command[1].ToUpper());

                    currentDate = DateTime.Today;
                    currentCurrency = string.Empty;

                    await BotMessage.SendMessageMarkdown(botClient, chatId, $"*{currentBank.Name}*\nPress *Date* button to select the date");
                    break;

                case BotCommands.CMD_DATE:
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
                            case BotCommands.PARAM_YEAR_DECREMENT:
                                currentDate = currentDate.AddYears(-1);
                                break;
                            case BotCommands.PARAM_YEAR_INCREMENT:
                                currentDate = currentDate.AddYears(1);
                                break;
                            case BotCommands.PARAM_MONTH_DECREMENT:
                                currentDate = currentDate.AddMonths(-1);
                                break;
                            case BotCommands.PARAM_MONTH_INCREMENT:
                                currentDate = currentDate.AddMonths(1);
                                break;
                            default:
                                isDayButtonPressed = false;
                                break;
                        }
                    }
                    
                    await BotMessage.SendAnswerCallbackQuery(botClient, update.CallbackQuery.Id, currentDate.ToString(currentBank.DateFormat));

                    if (isDayButtonPressed)
                    {
                        await BotMessage.EditMessage(
                            botClient,
                            update.CallbackQuery.Message.Chat.Id,
                            update.CallbackQuery.Message.MessageId,
                            ReplyKeyboard.InlineDateKeyboard(currentDate));
                    }
                    break;

                case BotCommands.CMD_DATECONFIRM:
                    currentCurrency = string.Empty;

                    ratesSource = JsonRatesParse.Parse(currentBank, currentDate);
                    currencyList = new CurrencyListServiceModel(ratesSource);

                    if (currencyList.Currencies.Count == 0)
                    {
                        await BotMessage.SendMessage(botClient, chatId, $"No currency rates on {currentDate.ToString(currentBank.DateFormat)}");
                        break;
                    }
                    await BotMessage.SendMessage(botClient, chatId, $"Select {currentDate.ToString(currentBank.DateFormat)}");

                    IsDateSelected = true;

                    await BotMessage.SendMessageMarkdown(botClient, chatId, "Press *Currency* button to select the currency");
                    break;

                case BotCommands.CMD_CURRENCY:
                    await BotMessage.SendAnswerCallbackQuery(botClient, update.CallbackQuery.Id, command[1]);

                    currentCurrency = currencyList?.Currencies.Find(c => !string.IsNullOrWhiteSpace(c) && c.ToUpper() == command[1].ToUpper());

                    CurrencyRateServiceModel currencyRate = new(ratesSource, currentCurrency);
                    ReportModel rep = new(currencyRate);

                    await BotMessage.SendMessage(botClient, chatId, rep.Report);
                    break;
            }
        }
    }
}
