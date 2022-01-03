using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Models;
using TelegramBot.Services;

namespace TelegramBot.Handlers
{
    public class CommandDateHandler
    {
        public static async Task Handler(ITelegramBotClient botClient, Message message, string cmd, CurrentSession current)
        {
            var chatId = message.Chat.Id;
            var messageText = message.Text;
            string[] command = cmd.Split(" ");

            if (current.Bank == null)
            {
                await BotMessage.SendMessage(botClient, chatId, "Select the bank");
                return;
            }

            if (command.Length == 1)
            {
                await BotMessage.SendMessageMarkdownKeyboard(
                    botClient,
                    chatId,
                    "Select the date",
                    ReplyKeyboard.InlineDateKeyboard(current.Date));
                return;
            }

            bool isDayButtonPressed = true;
            if (int.TryParse(command[1], out int buttonValue))
            {
                if (buttonValue is >= 1 and <= 31)
                {
                    current.Date = new DateTime(current.Date.Year, current.Date.Month, buttonValue);
                }
            }
            else
            {
                switch (command[1])
                {
                    case Services.BotCommand.PARAM_YEAR_DECREMENT:
                        current.Date = current.Date.AddYears(-1);
                        break;
                    case Services.BotCommand.PARAM_YEAR_INCREMENT:
                        current.Date = current.Date.AddYears(1);
                        break;
                    case Services.BotCommand.PARAM_MONTH_DECREMENT:
                        current.Date = current.Date.AddMonths(-1);
                        break;
                    case Services.BotCommand.PARAM_MONTH_INCREMENT:
                        current.Date = current.Date.AddMonths(1);
                        break;
                    default:
                        isDayButtonPressed = false;
                        break;
                }
            }

            if (isDayButtonPressed)
            {
                await BotMessage.EditMessage(
                    botClient,
                    chatId,
                    message.MessageId,
                    ReplyKeyboard.InlineDateKeyboard(current.Date));
            }
        }
    }
}
