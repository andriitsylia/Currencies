using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services;

namespace TelegramBot.Handlers
{
    public class CommandHelpHandler
    {
        public static async Task Handler(ITelegramBotClient botClient, Message message)
        {
            string usage = "*Bot usage:*\n"
                           + "/start \\- restart the bot\n"
                           + "/bank \\- show banks list\n"
                           + "/date \\- show date picker\n"
                           + "/currency \\- show currency list\n"
                           + "/help";

            await BotMessage.SendMessageMarkdown(botClient, message.Chat.Id, usage);
        }
    }
}
