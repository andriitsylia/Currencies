using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Services
{
    public class BotMessage
    {

        public static async Task SendMessage(ITelegramBotClient botClient, ChatId chatId, string message)
        {
            _ = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message);
        }

        public static async Task SendMessageMarkdown(ITelegramBotClient botClient, ChatId chatId, string message)
        {
            _ = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }

        public static async Task SendMessageKeyboard(ITelegramBotClient botClient, ChatId chatId, string message, IReplyMarkup keyboard)
        {
            _ = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message,
                replyMarkup:keyboard);
        }

        public static async Task SendMessageMarkdownKeyboard(ITelegramBotClient botClient, ChatId chatId, string message, IReplyMarkup keyboard)
        {
            _ = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
                replyMarkup: keyboard);
        }

        public static async Task SendAnswerCallbackQuery(ITelegramBotClient botClient, string callbackQueryId, string message = null)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId, message);
        }

        public static async Task EditMessage(ITelegramBotClient botClient, ChatId chatId, int messageId, InlineKeyboardMarkup keyboard)
        {
            _ = await botClient.EditMessageReplyMarkupAsync(
                chatId,
                messageId,
                keyboard);
        }
    }
}