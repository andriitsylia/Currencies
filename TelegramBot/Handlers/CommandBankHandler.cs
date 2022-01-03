using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Models;
using TelegramBot.Services;
using TelegramBot.Settings;

namespace TelegramBot.Handlers
{
    public class CommandBankHandler
    {
        public static async Task Handler(ITelegramBotClient botClient, Message message, string cmd, CurrentSession currentSession)
        {
            var chatId = message.Chat.Id;
            var messageText = message.Text;
            string[] command = cmd.Split(" ");

            Banks banks = new BanksSettings().Get();

            if (banks == null)
            {
                await BotMessage.SendMessage(botClient, chatId, "Banks settings is not loaded");
                return;
            }

            if (command.Length == 1)
            {
                await BotMessage.SendMessageKeyboard(
                    botClient,
                    chatId,
                    "Select the bank",
                    ReplyKeyboard.InlineBanksKeyboard(banks));

                return;
            }
            
            currentSession.Bank = banks.GetBank(command[1]);

            if (currentSession.Bank != null)
            {
                await BotMessage.SendMessageMarkdown(botClient, chatId, $"Bank *{currentSession.Bank.Name}* is selected");
            }
            else
            {
                await BotMessage.SendMessageMarkdown(botClient, chatId, $"Bank *{command[1].ToUpper()}* is not exist");
            }

        }
    }
}
