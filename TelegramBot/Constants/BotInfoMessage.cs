using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Constants
{
    public class BotInfoMessage
    {
        public const string BOT_CONSOLE_TITLE = "Telegram currency bot";
        public const string BOT_LISTENING = "Start listening for @";

        public const string START = "Select the *Bank*, the *Date* and the *Currency*";

        public const string BANK_SELECT = "Select the bank";
        public const string BANK_SELECTED = ": bank is selected";
        public const string BANK_NOT_LOADED = "Banks settings is not loaded";
        public const string BANK_NOT_EXIST = ": bank is not exist";

        public const string CURRENCY_SELECT = "Select the currency";
        public const string CURRENCY_NOT_EXIST = ": currency is not exist";
        public const string CURRENCY_NO_RATES = "No currency rates on ";
        
        public const string DATE_SELECT = "Select the date";
        public const string DATE_SELECTED = ": date is selected";
        public const string DATE_NOT_VALID = ": date is not valid";
        
        public const string HELP = "*Bot usage:*\n"
                                 + "/start \\- restart the bot\n"
                                 + "/bank \\- banks list\n"
                                 + "/bank _bank_ \\- select the bank\n"
                                 + "/date \\- show date picker\n"
                                 + "/date _date_ \\- select the date\n"
                                 + "/currency \\- show currency list\n"
                                 + "/currency _currency_ \\- select the currency\n"
                                 + "/help";

        public const string SPLIT_CHAR = " ";
        public const string NULL_ARGUMENT = "Received a null argument";
    }
}
