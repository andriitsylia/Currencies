using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services;

namespace TelegramBot.Handlers
{
    public class CommandModeHandler
    {
        public static async Task Handler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "Please, select a bot mode:",
                replyMarkup: ReplyKeyboard.ModeKeyboard(),
                cancellationToken: cancellationToken);
        }
    }
}
