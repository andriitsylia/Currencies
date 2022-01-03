using System;
using System.Collections.Generic;
using System.Globalization;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Models;
using TelegramBot.Settings;

namespace TelegramBot.Services
{
    public class ReplyKeyboard
    {
        private const int BUTTONS_IN_ROW = 5;
        private const int BUTTONS_IN_WEEK = 7;

        public static ReplyKeyboardMarkup MainKeyboard()
        {
            return new(new[]
            {
                new KeyboardButton[] {
                    BotCommand.BUTTON_BANK,
                    BotCommand.BUTTON_DATE,
                    BotCommand.BUTTON_CURRENCY,
                    BotCommand.BUTTON_HELP
                }
            })
            {
                ResizeKeyboard = true
            };
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
                    {
                        InlineKeyboardButton.WithCallbackData(text: bank.Name, callbackData: $"{BotCommand.CMD_BANK} {bank.Name}")
                    });
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
                    InlineKeyboardButton.WithCallbackData(
                        text: BotCommand.BUTTON_DECREMENT,
                        callbackData: $"{BotCommand.CMD_DATE} {BotCommand.PARAM_YEAR_DECREMENT}"),
                    InlineKeyboardButton.WithCallbackData(
                        text: date.Year.ToString(),
                        callbackData: $"{BotCommand.CMD_DATE} {BotCommand.PARAM_YEAR}"),
                    InlineKeyboardButton.WithCallbackData(
                        text: BotCommand.BUTTON_INCREMENT,
                        callbackData: $"{BotCommand.CMD_DATE} {BotCommand.PARAM_YEAR_INCREMENT}")
                },
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData(
                        text: BotCommand.BUTTON_DECREMENT,
                        callbackData: $"{BotCommand.CMD_DATE} {BotCommand.PARAM_MONTH_DECREMENT}"),
                    InlineKeyboardButton.WithCallbackData(
                        text: DateTimeFormatInfo.CurrentInfo.MonthNames[date.Month - 1],
                        callbackData: $"{BotCommand.CMD_DATE} {BotCommand.PARAM_MONTH}"),
                    InlineKeyboardButton.WithCallbackData(
                        text: BotCommand.BUTTON_INCREMENT,
                        callbackData: $"{BotCommand.CMD_DATE} {BotCommand.PARAM_MONTH_INCREMENT}")
                }
            };

            row = new List<InlineKeyboardButton>();
            int days = DateTime.DaysInMonth(date.Year, date.Month);
            int day = (int)new DateTime(date.Year, date.Month, 1).DayOfWeek;
            for (int i = 1; i < (day == 0 ? 7 : day); i++)
            {
                row.Add(InlineKeyboardButton.WithCallbackData(
                    text: BotCommand.BUTTON_EMPTY,
                    callbackData: $"{BotCommand.CMD_DATE} {BotCommand.PARAM_EMPTY}"));
            }
            for (int i = 1; i <= days; i++)
            {
                if (row.Count == BUTTONS_IN_WEEK)
                {
                    keyboard.Add(row);
                    row = new List<InlineKeyboardButton>();
                }
                row.Add(InlineKeyboardButton.WithCallbackData(
                    text: i.ToString(),
                    callbackData: $"{BotCommand.CMD_DATE} {i}"));
            }
            for (int i = row.Count + 1; i <= BUTTONS_IN_WEEK; i++)
            {
                row.Add(InlineKeyboardButton.WithCallbackData(
                    text: BotCommand.BUTTON_EMPTY,
                    callbackData: $"{BotCommand.CMD_DATE} {BotCommand.PARAM_EMPTY}"));
            }
            keyboard.Add(row);

            row = new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData(
                    text: $"{BotCommand.BUTTON_DATECONFIRM} {date:d}",
                    callbackData: BotCommand.CMD_DATECONFIRM)
            };
            keyboard.Add(row);

            inlineKeyboardMarkup = new(keyboard);

            return inlineKeyboardMarkup;
        }

        public static InlineKeyboardMarkup InlineCurrencyKeyboard(CurrencyListModel currencyList)
        {
            InlineKeyboardMarkup inlineKeyboardMarkup;
            List<InlineKeyboardButton> row = new();
            List<List<InlineKeyboardButton>> keyboard = new();
            foreach (var currency in currencyList.Currencies)
            {
                if (row.Count == BUTTONS_IN_ROW)
                {
                    keyboard.Add(row);
                    row = new List<InlineKeyboardButton>();
                }
                row.Add(InlineKeyboardButton.WithCallbackData(
                    text: currency.ToString(),
                    callbackData: $"{BotCommand.CMD_CURRENCY} {currency}"));
            }
            keyboard.Add(row);

            inlineKeyboardMarkup = new(keyboard);

            return inlineKeyboardMarkup;
        }

    }
}

