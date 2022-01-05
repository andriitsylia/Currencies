using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Constants;
using TelegramBot.Models;
using TelegramBot.Services;

namespace TelegramBot.Handlers
{
    public class CommandDateConfirmHandler
    {
        public static async Task Handler(ITelegramBotClient botClient, Message message, CurrentSession currentSession)
        {
            await BotMessage.SendMessage(
                botClient,
                message.Chat.Id,
                currentSession.Date.ToString(currentSession.Bank.DateFormat) + BotInfoMessage.DATE_SELECTED);
        }
    }
}
