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
        public static async Task Handler(ITelegramBotClient botClient, Message message, string cmd, CurrentSession currentSession)
        {
            var chatId = message.Chat.Id;
            var messageText = message.Text;
            string[] command = cmd.Split(" ");

            if (currentSession.Bank == null)
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
                    ReplyKeyboard.InlineDateKeyboard(currentSession.Date));
                return;
            }

            bool isDayButtonPressed = false;
            if (int.TryParse(command[1], out int buttonValue))
            {
                if (buttonValue is >= 1 and <= 31)
                {
                    currentSession.Date = new DateTime(currentSession.Date.Year, currentSession.Date.Month, buttonValue);
                    isDayButtonPressed = true;
                }
            }
            else
            {
                switch (command[1])
                {
                    case Services.BotCommand.PARAM_YEAR_DECREMENT:
                        currentSession.Date = currentSession.Date.AddYears(-1);
                        isDayButtonPressed=true;
                        break;
                    case Services.BotCommand.PARAM_YEAR_INCREMENT:
                        currentSession.Date = currentSession.Date.AddYears(1);
                        isDayButtonPressed = true;
                        break;
                    case Services.BotCommand.PARAM_MONTH_DECREMENT:
                        currentSession.Date = currentSession.Date.AddMonths(-1);
                        isDayButtonPressed = true;
                        break;
                    case Services.BotCommand.PARAM_MONTH_INCREMENT:
                        currentSession.Date = currentSession.Date.AddMonths(1);
                        isDayButtonPressed = true;
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
                    ReplyKeyboard.InlineDateKeyboard(currentSession.Date));
            }
            else
            {
                if (DateTime.TryParse(command[1], out DateTime date))
                {
                    currentSession.Date = date;
                    await BotMessage.SendMessage(
                        botClient,
                        message.Chat.Id,
                        $"Date {currentSession.Date.ToString(currentSession.Bank.DateFormat)} is selected");
                }
                else
                {
                    await BotMessage.SendMessage(botClient, chatId, $"Date {command[1]} is not valid");
                }
            }
        }
    }
}
