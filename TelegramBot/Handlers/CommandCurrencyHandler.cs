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
        public static PrivatBankRatesSourceModel ratesSource;
        public static CurrencyListServiceModel currencyList;

        public static async Task Handler(ITelegramBotClient botClient, Message message, string cmd, CurrentSession current)
        {
            var chatId = message.Chat.Id;
            string[] command = cmd.Split(" ");

            if (command.Length == 1)
            {
                ratesSource = JsonRatesParse.Parse(current.Bank, current.Date); 
                currencyList = new CurrencyListServiceModel(ratesSource);

                if (currencyList.Currencies.Count == 0)
                {
                    await BotMessage.SendMessage(botClient, chatId, $"No currency rates on {current.Date.ToString(current.Bank.DateFormat)}");
                    return;
                }

                await BotMessage.SendMessageKeyboard(
                    botClient,
                    chatId,
                    "Select the currency",
                    ReplyKeyboard.InlineCurrencyKeyboard(currencyList));

                return;
            }

            current.Currency = currencyList.GetCurrency(command[1]);

            if (!string.IsNullOrWhiteSpace(current.Currency))
            {
                CurrencyRateServiceModel currencyRate = new(ratesSource, current.Currency);
                ReportModel rep = new(currencyRate);
                await BotMessage.SendMessage(botClient, chatId, rep.Report);
            }
            else
            {
                await BotMessage.SendMessageMarkdown(botClient, chatId, $"Currency _{command[1]}_ is not present");
            }
        }
    }
}
