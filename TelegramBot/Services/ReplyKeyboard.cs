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

        public static ReplyKeyboardMarkup ReplyMainKeyboard()
        {
            return new(new[]
            {
                new KeyboardButton[] {"Banks", "Currency", "Date"}
            })
            { ResizeKeyboard = true };
        }

        public static ReplyKeyboardMarkup ReplyCurrencyKeyboard(PrivatBankCurrencyListServiceModel privatBankCurrencyList)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup;
            List<KeyboardButton> row = new();
            List<List<KeyboardButton>> rows = new();
            foreach (var currency in privatBankCurrencyList.Currencies)
            {
                if (!string.IsNullOrWhiteSpace(currency))
                {
                    if (row.Count == 5)
                    {
                        rows.Add(row);
                        row = new List<KeyboardButton>();
                    }
                    row.Add(currency);
                }
            }
            rows.Add(row);
            replyKeyboardMarkup = new(rows) { ResizeKeyboard = true };

            return replyKeyboardMarkup;
        }

        public static InlineKeyboardMarkup InlineCurrencyKeyboard(PrivatBankCurrencyListServiceModel privatBankCurrencyList)
        {
            InlineKeyboardMarkup inlineKeyboardMarkup;
            List<InlineKeyboardButton> row = new();
            List<List<InlineKeyboardButton>> rows = new();
            foreach (var currency in privatBankCurrencyList.Currencies)
            {
                if (!string.IsNullOrWhiteSpace(currency))
                {
                    if (row.Count == 5)
                    {
                        rows.Add(row);
                        row = new List<InlineKeyboardButton>();
                    }
                    row.Add(InlineKeyboardButton.WithCallbackData(text: currency, callbackData: "/currency " + currency));
                }
            }
            rows.Add(row);
            row = new List<InlineKeyboardButton>();
            row.Add(InlineKeyboardButton.WithCallbackData(text: "Hide currency keyboard", callbackData: "/hide_currency_keyboard"));
            rows.Add(row);
            inlineKeyboardMarkup = new(rows);

            return inlineKeyboardMarkup;
        }
    }
}
