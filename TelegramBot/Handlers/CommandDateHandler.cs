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
    public class CommandDateHandler
    {
        public static async Task Handler(ITelegramBotClient botClient, Message message, string cmd)
        {
            var chatId = message.Chat.Id;
            var messageText = message.Text;
            string[] command = cmd.Split(" ");

            if (MessageHandler.currentBank == null)
            {
                await BotMessage.SendMessageMarkdown(botClient, chatId, "Select the bank");
                return;
            }

            if (command.Length == 1)
            {
                await BotMessage.SendMessageMarkdownKeyboard(
                    botClient,
                    chatId,
                    "Select the date",
                    ReplyKeyboard.InlineDateKeyboard(MessageHandler.currentDate));
                return;
            }

            //if (DateTime.TryParse(command[1], out MessageHandler.currentDate))
            //{
            //    //MessageHandler.currentCurrency = string.Empty;

            //    MessageHandler.ratesSource = JsonRatesParse.Parse(MessageHandler.currentBank, MessageHandler.currentDate);
            //    MessageHandler.currencyList = new CurrencyListServiceModel(MessageHandler.ratesSource);

            //    if (MessageHandler.currencyList.Currencies.Count == 0)
            //    {
            //        await BotMessage.SendMessage(
            //            botClient,
            //            chatId,
            //            $"No currency rates on {MessageHandler.currentDate.ToString(MessageHandler.currentBank.DateFormat)}");
            //        return;
            //    }

            //    MessageHandler.IsDateSelected = true;
            //    //await BotMessage.SendMessageMarkdown(
            //    //    botClient,
            //    //    chatId,
            //    //    $"Please, select any of the following currency :\n {string.Join(" ", MessageHandler.currencyList.Currencies)}\n\nType */currency* _currency_");
            //}
            //else
            //{
            //    await BotMessage.SendMessageMarkdown(botClient, chatId, "The date isn't in _dd\\.mm\\.yyyy_ format");
            //}

            bool isDayButtonPressed = true;
            if (int.TryParse(command[1], out int buttonValue))
            {
                if (buttonValue is >= 1 and <= 31)
                {
                    MessageHandler.currentDate = new DateTime(MessageHandler.currentDate.Year, MessageHandler.currentDate.Month, buttonValue);
                }
                //isDayButtonPressed = false;
            }
            else
            {
                switch (command[1])
                {
                    case BotCommands.PARAM_YEAR_DECREMENT:
                        MessageHandler.currentDate = MessageHandler.currentDate.AddYears(-1);
                        break;
                    case BotCommands.PARAM_YEAR_INCREMENT:
                        MessageHandler.currentDate = MessageHandler.currentDate.AddYears(1);
                        break;
                    case BotCommands.PARAM_MONTH_DECREMENT:
                        MessageHandler.currentDate = MessageHandler.currentDate.AddMonths(-1);
                        break;
                    case BotCommands.PARAM_MONTH_INCREMENT:
                        MessageHandler.currentDate = MessageHandler.currentDate.AddMonths(1);
                        break;
                    default:
                        isDayButtonPressed = false;
                        break;
                }
            }

            //await BotMessage.SendAnswerCallbackQuery(botClient, callbackQuery.Id, MessageHandler.currentDate.ToString(MessageHandler.currentBank.DateFormat));

            if (isDayButtonPressed)             {
                await BotMessage.EditMessage(
                    botClient,
                    chatId,
                    message.MessageId,
                    ReplyKeyboard.InlineDateKeyboard(MessageHandler.currentDate));
            }
        }
    }
}
