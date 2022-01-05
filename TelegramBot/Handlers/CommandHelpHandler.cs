using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Constants;
using TelegramBot.Services;

namespace TelegramBot.Handlers
{
    public class CommandHelpHandler
    {
        public static async Task Handler(ITelegramBotClient botClient, Message message)
        {
            await BotMessage.SendMessageMarkdown(botClient, message.Chat.Id, BotInfoMessage.HELP);
        }
    }
}
