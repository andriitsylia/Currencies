﻿using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Models;
using TelegramBot.Services;

namespace TelegramBot.Handlers
{
    public class CommandDateConfirmHandler
    {
        public static async Task Handler(ITelegramBotClient botClient, Message message, string cmd, CurrentSession currentSession)
        {
            await BotMessage.SendMessage(
                botClient,
                message.Chat.Id,
                $"Date {currentSession.Date.ToString(currentSession.Bank.DateFormat)} is selected");
        }
    }
}
