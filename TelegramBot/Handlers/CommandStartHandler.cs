using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
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
                        "Select the *Bank*, the *Date* and the *Currency*",
                        ReplyKeyboard.MainKeyboard());
        }
    }
}
