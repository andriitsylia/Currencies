using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Constants;
using TelegramBot.Services;

namespace TelegramBot.Handlers
{
    public class CommandStartHandler
    {

        public static async Task Handler(ITelegramBotClient botClient, Message message)
        {
            await BotMessage.SendMessageMarkdownKeyboard(
                        botClient,
                        message.Chat.Id,
                        BotInfoMessage.START,
                        ReplyKeyboard.MainKeyboard());
        }
    }
}
