using System;
using System.Linq;

namespace TelegramBot.Services
{
    public class BotCommand
    {
        public const string CMD_START = "/start";
        public const string CMD_BANK = "/bank";
        public const string CMD_DATE = "/date";
        public const string CMD_DATECONFIRM = "/dateconfirm";
        public const string CMD_CURRENCY = "/currency";
        public const string CMD_HELP = "/help";

        public const string PARAM_DEFAULT = "default";
        public const string PARAM_TODAY = "today";
        public const string PARAM_EMPTY = "empty";
        public const string PARAM_YEAR = "year";
        public const string PARAM_YEAR_DECREMENT = "year-";
        public const string PARAM_YEAR_INCREMENT = "year+";
        public const string PARAM_MONTH = "month";
        public const string PARAM_MONTH_DECREMENT = "month-";
        public const string PARAM_MONTH_INCREMENT = "month+";

        public const string BUTTON_BANK = "Bank";
        public const string BUTTON_DATE = "Date";
        public const string BUTTON_CURRENCY = "Currency";
        public const string BUTTON_EMPTY = " ";
        public const string BUTTON_DECREMENT = "-";
        public const string BUTTON_INCREMENT = "+";
        public const string BUTTON_DATECONFIRM = "Confirm date";

        private static string[] commands = {
            CMD_START,
            CMD_BANK,
            CMD_DATE,
            CMD_DATECONFIRM,
            CMD_CURRENCY,
            CMD_HELP
        };

        private static string[] parameters = {
            PARAM_DEFAULT,
            PARAM_TODAY,
            PARAM_EMPTY,
            PARAM_YEAR,
            PARAM_YEAR_DECREMENT,
            PARAM_YEAR_INCREMENT,
            PARAM_MONTH,
            PARAM_MONTH_DECREMENT,
            PARAM_MONTH_INCREMENT
        };

        private static string[] buttons = {
            BUTTON_BANK,
            BUTTON_DATE,
            BUTTON_CURRENCY,
            BUTTON_EMPTY,
            BUTTON_DECREMENT,
            BUTTON_INCREMENT,
            BUTTON_DATECONFIRM
        };

        private static bool IsCommand(string command) => commands.Contains(command.Trim());

        private static bool IsParameter(string parameter) => parameters.Contains(parameter.Trim());

        private static bool IsButton(string button) => buttons.Contains(button.Trim());

        public static bool IsValidCommand(string command)
        {
            return true;
        }
    }
}
