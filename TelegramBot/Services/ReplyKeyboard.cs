using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Models;

namespace TelegramBot.Services
{
    public class ReplyKeyboard
    {
        private const int BUTTONS_IN_ROW = 5;
        public static ReplyKeyboardMarkup ReplyMainKeyboard()
        {
            return new(new[]
            {
                new KeyboardButton[] {"Banks", "Currency", "Date"},
                new KeyboardButton[]{"Hide main keyboard"}
            })
            { ResizeKeyboard = true };
        }

        public static ReplyKeyboardMarkup ReplyCurrencyKeyboard(PrivatBankCurrencyListServiceModel privatBankCurrencyList)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup;
            List<KeyboardButton> row = new();
            List<List<KeyboardButton>> keyboard = new();
            foreach (var currency in privatBankCurrencyList.Currencies)
            {
                if (!string.IsNullOrWhiteSpace(currency))
                {
                    if (row.Count == BUTTONS_IN_ROW)
                    {
                        keyboard.Add(row);
                        row = new List<KeyboardButton>();
                    }
                    row.Add(currency);
                }
            }
            keyboard.Add(row);
            replyKeyboardMarkup = new(keyboard) { ResizeKeyboard = true };

            return replyKeyboardMarkup;
        }

        public static InlineKeyboardMarkup InlineCurrencyKeyboard(PrivatBankCurrencyListServiceModel privatBankCurrencyList)
        {
            InlineKeyboardMarkup inlineKeyboardMarkup;
            List<InlineKeyboardButton> row = new();
            List<List<InlineKeyboardButton>> keyboard = new();
            foreach (var currency in privatBankCurrencyList.Currencies)
            {
                if (!string.IsNullOrWhiteSpace(currency))
                {
                    if (row.Count == BUTTONS_IN_ROW)
                    {
                        keyboard.Add(row);
                        row = new List<InlineKeyboardButton>();
                    }
                    row.Add(InlineKeyboardButton.WithCallbackData(text: currency, callbackData: "/currency " + currency));
                }
            }
            keyboard.Add(row);
            row = new List<InlineKeyboardButton>();
            row.Add(InlineKeyboardButton.WithCallbackData(text: "Hide currency keyboard", callbackData: "/hide_currency_keyboard"));
            keyboard.Add(row);
            inlineKeyboardMarkup = new(keyboard);

            return inlineKeyboardMarkup;
        }
    }
}
