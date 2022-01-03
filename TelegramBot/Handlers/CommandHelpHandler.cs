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
                           + "/bank \\- banks list\n"
                           + "/bank _bank_ \\- select the bank\n"
                           + "/date \\- show date picker\n"
                           + "/date _date_ \\- select the date\n"
                           + "/currency \\- show currency list\n"
                           + "/currency _currency_ \\- select the currency\n"
                           + "/help";

            await BotMessage.SendMessageMarkdown(botClient, message.Chat.Id, usage);
        }
    }
}
