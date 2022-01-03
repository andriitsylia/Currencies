using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
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
            string[] command = cmd.Split(" ");

            if (command.Length == 1)
            {
                rates = new BankRates(currentSession.Bank).Get(currentSession.Date).Result;
                currencyList = new CurrencyListModel(rates);

                if (currencyList.Currencies.Count == 0)
                {
                    await BotMessage.SendMessage(
                        botClient,
                        chatId,
                        $"No currency rates on {currentSession.Date.ToString(currentSession.Bank.DateFormat)}");
                    return;
                }

                await BotMessage.SendMessageKeyboard(
                    botClient,
                    chatId,
                    "Select the currency",
                    ReplyKeyboard.InlineCurrencyKeyboard(currencyList));

                return;
            }

            currentSession.Currency = currencyList.GetCurrency(command[1]);

            if (!string.IsNullOrWhiteSpace(currentSession.Currency))
            {
                ReportModel rep = new(rates.GetRate(currentSession.Currency));
                await BotMessage.SendMessage(botClient, chatId, rep.Report);
            }
            else
            {
                await BotMessage.SendMessageMarkdown(botClient, chatId, $"Currency _{command[1]}_ is not present");
            }
        }
    }
}
