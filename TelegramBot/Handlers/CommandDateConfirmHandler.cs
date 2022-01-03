using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Models;
using TelegramBot.Services;

namespace TelegramBot.Handlers
{
    public class CommandDateConfirmHandler
    {
        public static PrivatBankRatesSourceModel ratesSource;
        public static CurrencyListServiceModel currencyList;

        public static async Task Handler(ITelegramBotClient botClient, Message message, string cmd, CurrentSession current)
        {
            await BotMessage.SendMessage(
                botClient,
                message.Chat.Id,
                $"{current.Date.ToString(current.Bank.DateFormat)} is selected");
        }
    }
}
