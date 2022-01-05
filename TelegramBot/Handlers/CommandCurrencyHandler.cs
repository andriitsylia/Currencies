using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Constants;
using TelegramBot.Models;
using TelegramBot.Services;

namespace TelegramBot.Handlers
{
    public class CommandCurrencyHandler
    {
        public static RatesModel rates;
        public static CurrencyListModel currencyList;

        public static async Task Handler(ITelegramBotClient botClient, Message message, string cmd, CurrentSession currentSession)
        {
            var chatId = message.Chat.Id;
            string[] command = cmd.Split(BotInfoMessage.SPLIT_CHAR);

            if (command.Length == 1)
            {
                rates = new BankRates(currentSession.Bank).Get(currentSession.Date).Result;
                currencyList = new CurrencyListModel(rates);

                if (currencyList.Currencies.Count == 0)
                {
                    await BotMessage.SendMessage(
                        botClient,
                        chatId,
                        BotInfoMessage.CURRENCY_NO_RATES + currentSession.Date.ToString(currentSession.Bank.DateFormat));
                    return;
                }

                await BotMessage.SendMessageKeyboard(
                    botClient,
                    chatId,
                    BotInfoMessage.CURRENCY_SELECT,
                    ReplyKeyboard.InlineCurrencyKeyboard(currencyList));

                return;
            }
            if (currencyList.IsValid(command[1]))
            {
                currentSession.Currency = currencyList.Get(command[1]);
                ReportModel report = new(rates.GetRate(currentSession.Currency));
                await BotMessage.SendMessage(botClient, chatId, report.Report);
            }
            else
            {
                await BotMessage.SendMessageMarkdown(botClient, chatId, command[1].ToUpper() + BotInfoMessage.CURRENCY_NOT_EXIST);
            }
        }
    }
}
