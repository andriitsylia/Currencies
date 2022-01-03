using System;
using TelegramBot.Settings;

namespace TelegramBot.Models
{
    public class CurrentSession
    {
        public Bank Bank { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }

        public CurrentSession()
        {
            Bank = null;
            Date = DateTime.Today;
            Currency = string.Empty;
        }
    }
}
