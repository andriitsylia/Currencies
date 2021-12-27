﻿using System;
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
                           + "/start \\- begin\\/restart work with the bot\n"
                           + "/bankslist\n"
                           + "/bank _bank_\n"
                           + "/date _dd\\.mm\\.yyyy_\n"
                           + "/currency _currency_\n"
                           + "/help";

            Message sentMessage = await botClient.SendTextMessageAsync(
                       chatId: message.Chat.Id,
                       text: usage,
                       parseMode: ParseMode.MarkdownV2,
                       replyMarkup: null);
        }

    }
}
