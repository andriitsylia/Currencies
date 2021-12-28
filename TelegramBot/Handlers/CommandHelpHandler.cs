using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Handlers
{
    public class CommandHelpHandler
    {
        public static async Task Handler(ITelegramBotClient botClient, Message message)
        {
            string usage = "*Bot usage:*\n"
                           + "/start \\- restart the bot\n"
                           + "/bank \\- show banks list\n"
                           + "/bank _bank_ \\- select bank\n"
                           + "/date _dd\\.mm\\.yyyy_ \\- select date\n"
                           + "/date _today_ \\- select date\n"
                           + "/currency _currency_ \\- select currency\n"
                           + "/help";

            Message sentMessage = await botClient.SendTextMessageAsync(
                       chatId: message.Chat.Id,
                       text: usage,
                       parseMode: ParseMode.MarkdownV2,
                       replyMarkup: null);
        }

    }
}
