using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Models;
using TelegramBot.Settings;

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

        public static ReplyKeyboardMarkup ReplyCurrencyKeyboard(CurrencyListServiceModel currencyList)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup;
            List<KeyboardButton> row = new();
            List<List<KeyboardButton>> keyboard = new();
            foreach (var currency in currencyList.Currencies)
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

        public static InlineKeyboardMarkup InlineBanksKeyboard(Banks banks)
        {
            InlineKeyboardMarkup inlineKeyboardMarkup;
            List<List<InlineKeyboardButton>> keyboard = new();
            foreach (var bank in banks.Items)
            {
                if (!string.IsNullOrWhiteSpace(bank.Name))
                {
                    keyboard.Add(new List<InlineKeyboardButton>()
                        { InlineKeyboardButton.WithCallbackData(text: bank.Name, callbackData: "/bank " + bank.Name) });
                }
            }
            inlineKeyboardMarkup = new(keyboard);

            return inlineKeyboardMarkup;
        }

        public static InlineKeyboardMarkup InlineCurrencyKeyboard(CurrencyListServiceModel currencyList)
        {
            InlineKeyboardMarkup inlineKeyboardMarkup;
            List<InlineKeyboardButton> row = new();
            List<List<InlineKeyboardButton>> keyboard = new();
            foreach (var currency in currencyList.Currencies)
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

        public static InlineKeyboardMarkup InlineDateKeyboard(DateTime date)
        {
            InlineKeyboardMarkup inlineKeyboardMarkup;
            List<InlineKeyboardButton> row;
            List<List<InlineKeyboardButton>> keyboard = new();

            row = new List<InlineKeyboardButton>();
            row.Add(InlineKeyboardButton.WithCallbackData(text: "-", callbackData: "/date year-"));
            row.Add(InlineKeyboardButton.WithCallbackData(text: date.Year.ToString(), callbackData: "/date year"));
            row.Add(InlineKeyboardButton.WithCallbackData(text: "+", callbackData: "/date year+"));
            keyboard.Add(row);

            row = new List<InlineKeyboardButton>();
            row.Add(InlineKeyboardButton.WithCallbackData(text: "-", callbackData: "/date month-"));
            row.Add(InlineKeyboardButton.WithCallbackData(text: DateTimeFormatInfo.CurrentInfo.MonthNames[date.Month-1], callbackData: "/date month"));
            row.Add(InlineKeyboardButton.WithCallbackData(text: "+", callbackData: "/date month+"));
            keyboard.Add(row);
            row = new List<InlineKeyboardButton>();
            int days = DateTime.DaysInMonth(date.Year, date.Month);
            int day = (int)date.DayOfWeek;
            for (int i = 1; i < (day == 0 ? 7: day); i++)
            {
                row.Add(InlineKeyboardButton.WithCallbackData(text: " ", callbackData: "/date 0"));
            }
            for (int i = 1; i <= days; i++)
            {
                if (row.Count == 7)
                {
                    keyboard.Add(row);
                    row = new List<InlineKeyboardButton>();
                }
                row.Add(InlineKeyboardButton.WithCallbackData(text: i.ToString(), callbackData: "/date " + i.ToString()));
            }
            for (int i = row.Count + 1; i <=7; i++)
            {
                row.Add(InlineKeyboardButton.WithCallbackData(text: " ", callbackData: "/date 0"));
            }
            keyboard.Add(row);

            inlineKeyboardMarkup = new(keyboard);

            return inlineKeyboardMarkup;
        }
    }
}

