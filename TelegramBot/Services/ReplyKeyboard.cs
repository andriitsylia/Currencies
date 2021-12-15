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
        private const int BUTTONS_IN_WEEK = 7;

        public static ReplyKeyboardMarkup ReplyMainKeyboard()
        {
            return new(new[]
            {
                new KeyboardButton[] {"Bank", "Date", "Currency"}
            })
            {
                ResizeKeyboard = true
            };
        }

        public static ReplyKeyboardMarkup ReplyModeKeyboard()
        {
            return new(new[]
            {
                new KeyboardButton[] {"Text", "Button"}
            })
            { ResizeKeyboard = true };
        }

        //public static ReplyKeyboardMarkup ReplyCurrencyKeyboard(CurrencyListServiceModel currencyList)
        //{
        //    ReplyKeyboardMarkup replyKeyboardMarkup;
        //    List<KeyboardButton> row = new();
        //    List<List<KeyboardButton>> keyboard = new();
        //    foreach (var currency in currencyList.Currencies)
        //    {
        //        if (!string.IsNullOrWhiteSpace(currency))
        //        {
        //            if (row.Count == BUTTONS_IN_ROW)
        //            {
        //                keyboard.Add(row);
        //                row = new List<KeyboardButton>();
        //            }
        //            row.Add(currency);
        //        }
        //    }
        //    keyboard.Add(row);
        //    replyKeyboardMarkup = new(keyboard) { ResizeKeyboard = true };

        //    return replyKeyboardMarkup;
        //}

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

        public static InlineKeyboardMarkup InlineDateKeyboard(DateTime date)
        {
            InlineKeyboardMarkup inlineKeyboardMarkup;
            List<InlineKeyboardButton> row;
            List<List<InlineKeyboardButton>> keyboard;

            keyboard = new()
            {
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData(text: "-", callbackData: "/date year-"),
                    InlineKeyboardButton.WithCallbackData(text: date.Year.ToString(), callbackData: "/date year"),
                    InlineKeyboardButton.WithCallbackData(text: "+", callbackData: "/date year+")
                },
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData(text: "-", callbackData: "/date month-"),
                    InlineKeyboardButton.WithCallbackData(text: DateTimeFormatInfo.CurrentInfo.MonthNames[date.Month - 1], callbackData: "/date month"),
                    InlineKeyboardButton.WithCallbackData(text: "+", callbackData: "/date month+")
                }
            };

            row = new List<InlineKeyboardButton>();
            int days = DateTime.DaysInMonth(date.Year, date.Month);
            int day = (int)new DateTime(date.Year, date.Month, 1).DayOfWeek;
            for (int i = 1; i < (day == 0 ? 7: day); i++)
            {
                row.Add(InlineKeyboardButton.WithCallbackData(text: " ", callbackData: "/date 0"));
            }
            for (int i = 1; i <= days; i++)
            {
                if (row.Count == BUTTONS_IN_WEEK)
                {
                    keyboard.Add(row);
                    row = new List<InlineKeyboardButton>();
                }
                row.Add(InlineKeyboardButton.WithCallbackData(text: i.ToString(), callbackData: "/date " + i.ToString()));
            }
            for (int i = row.Count + 1; i <= BUTTONS_IN_WEEK; i++)
            {
                row.Add(InlineKeyboardButton.WithCallbackData(text: " ", callbackData: "/date 0"));
            }
            keyboard.Add(row);

            row = new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData(text: "Confirm date", callbackData: "/confirmdate")
            };
            keyboard.Add(row);

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

            inlineKeyboardMarkup = new(keyboard);

            return inlineKeyboardMarkup;
        }

    }
}

