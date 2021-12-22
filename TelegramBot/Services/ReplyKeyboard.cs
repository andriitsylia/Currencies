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

        public static ReplyKeyboardMarkup MainKeyboard()
        {
            return new(new[]
            {
                new KeyboardButton[] {
                    BotCommands.BUTTON_BANK,
                    BotCommands.BUTTON_DATE,
                    BotCommands.BUTTON_CURRENCY
                }
            })
            {
                ResizeKeyboard = true
            };
        }

        public static ReplyKeyboardMarkup ModeKeyboard()
        {
            return new(new[]
            {
                new KeyboardButton[] {
                    BotCommands.BUTTON_TEXT,
                    BotCommands.BUTTON_BUTTON
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
                        InlineKeyboardButton.WithCallbackData(text: bank.Name, callbackData: BotCommands.CMD_BANK + " " + bank.Name)
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
                        text: BotCommands.BUTTON_DECREMENT,
                        callbackData: BotCommands.CMD_DATE + " " + BotCommands.PARAM_YEAR_DECREMENT),
                    InlineKeyboardButton.WithCallbackData(
                        text: date.Year.ToString(),
                        callbackData: BotCommands.CMD_DATE + " " + BotCommands.PARAM_YEAR),
                    InlineKeyboardButton.WithCallbackData(
                        text: BotCommands.BUTTON_INCREMENT,
                        callbackData: BotCommands.CMD_DATE + " " + BotCommands.PARAM_YEAR_INCREMENT)
                },
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData(
                        text: BotCommands.BUTTON_DECREMENT,
                        callbackData: BotCommands.CMD_DATE + " " + BotCommands.PARAM_MONTH_DECREMENT),
                    InlineKeyboardButton.WithCallbackData(
                        text: DateTimeFormatInfo.CurrentInfo.MonthNames[date.Month - 1],
                        callbackData: BotCommands.CMD_DATE + " " + BotCommands.PARAM_MONTH),
                    InlineKeyboardButton.WithCallbackData(
                        text: BotCommands.BUTTON_INCREMENT,
                        callbackData: BotCommands.CMD_DATE + " " + BotCommands.PARAM_MONTH_INCREMENT)
                }
            };

            row = new List<InlineKeyboardButton>();
            int days = DateTime.DaysInMonth(date.Year, date.Month);
            int day = (int)new DateTime(date.Year, date.Month, 1).DayOfWeek;
            for (int i = 1; i < (day == 0 ? 7 : day); i++)
            {
                row.Add(InlineKeyboardButton.WithCallbackData(
                    text: BotCommands.BUTTON_EMPTY,
                    callbackData: BotCommands.CMD_DATE + " " + BotCommands.PARAM_EMPTY));
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
                    callbackData: BotCommands.CMD_DATE + " " + i.ToString()));
            }
            for (int i = row.Count + 1; i <= BUTTONS_IN_WEEK; i++)
            {
                row.Add(InlineKeyboardButton.WithCallbackData(
                    text: BotCommands.BUTTON_EMPTY,
                    callbackData: BotCommands.CMD_DATE + " " + BotCommands.PARAM_EMPTY));
            }
            keyboard.Add(row);

            row = new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData(
                    text: BotCommands.BUTTON_DATECONFIRM,
                    callbackData: BotCommands.CMD_DATECONFIRM)
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
                    row.Add(InlineKeyboardButton.WithCallbackData(
                        text: currency,
                        callbackData: BotCommands.CMD_CURRENCY + " " + currency));
                }
            }
            keyboard.Add(row);

            inlineKeyboardMarkup = new(keyboard);

            return inlineKeyboardMarkup;
        }

    }
}

