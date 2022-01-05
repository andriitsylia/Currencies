using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Constants;
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
            string[] command = cmd.Split(BotInfoMessage.SPLIT_CHAR);

            Banks banks = new BanksSettings().Get();

            if (banks == null)
            {
                await BotMessage.SendMessage(botClient, chatId, BotInfoMessage.BANK_NOT_LOADED);
                return;
            }

            if (command.Length == 1)
            {
                await BotMessage.SendMessageKeyboard(
                    botClient,
                    chatId,
                    BotInfoMessage.BANK_SELECT,
                    ReplyKeyboard.InlineBanksKeyboard(banks));
                return;
            }

            currentSession.Bank = banks.GetBank(command[1]);

            if (currentSession.Bank != null)
            {
                await BotMessage.SendMessageMarkdown(botClient, chatId, currentSession.Bank.Name + BotInfoMessage.BANK_SELECTED);
            }
            else
            {
                await BotMessage.SendMessageMarkdown(botClient, chatId, command[1].ToUpper() + BotInfoMessage.BANK_NOT_EXIST);
            }
        }
    }
}
