using System;
using TelegramBot.Settings;

namespace TelegramBot.Models
{
    public class CurrentSession
    {
        public Bank Bank { get; set; }
        public DateTime Date { get; set; }
        public Currency Currency { get; set; }

        public CurrentSession()
        {
            Bank = null;
            Date = DateTime.Today;
            Currency = Currency.UAH;
        }
    }
}
