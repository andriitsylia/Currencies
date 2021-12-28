using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Services;

namespace TelegramBot.Handlers
{
    public class CommandBankHandler
    {
        public static async Task Handler(ITelegramBotClient botClient, Message message, string cmd)
        {
            var chatId = message.Chat.Id;
            var messageText = message.Text;
            string[] command = cmd.Split(" ");

            //if (_banks == null)
            //{
            //    await BotMessage.SendMessage(botClient, chatId, "Restart the bot\nType /start");
            //    return;
            //}

            if (command.Length == 1)
            {
                await botClient.SendChatActionAsync(chatId, ChatAction.Typing);

                await BotMessage.SendMessageKeyboard(
                    botClient,
                    chatId,
                    "Select the bank",
                    ReplyKeyboard.InlineBanksKeyboard(MessageHandler._banks));

                //await BotMessage.SendMessage(botClient, chatId, "Select the bank:\n");
                //await BotMessage.SendMessage(botClient, chatId, MessageHandler._banks.ToString());
                //await BotMessage.SendMessageMarkdown(botClient, chatId, "Type */bank* _bank_");
                return;
            }

            MessageHandler.currentBank = MessageHandler._banks.Items.FirstOrDefault(b => b.Name.ToUpper() == command[1].ToUpper());

            if (MessageHandler.currentBank != null)
            {
                await BotMessage.SendMessageMarkdown(botClient, chatId, "*" + MessageHandler.currentBank.Name + "*\n\nType */date* _dd\\.mm\\.yyyy_");
                MessageHandler.currentDate = DateTime.Today;
            }
            else
            {
                await BotMessage.SendMessageMarkdown(botClient, chatId, "I can't find the _" + command[1] + "_ bank");
            }

        }
    }
}
