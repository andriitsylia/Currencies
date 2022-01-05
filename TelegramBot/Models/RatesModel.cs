using System;
using System.Collections.Generic;
using TelegramBot.Constants;
using TelegramBot.Settings;

namespace TelegramBot.Models
{
    public class RatesModel
    {
        public Bank Bank { get; set; }
        public DateTime Date { get; set; }
        public List<RateModel> Rates { get; set; }

        public RatesModel(Bank bank, DateTime date)
        {
            Bank = bank ?? throw new ArgumentNullException(nameof(bank), BotInfoMessage.NULL_ARGUMENT);
            Date = date;
            Rates = new List<RateModel>();
        }

        public RateModel GetRate(Currency currenncy)
        {
            return Rates.Find(c => c.Currency == currenncy);
        }
    }
}
