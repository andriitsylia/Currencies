using System.Collections.Generic;

namespace TelegramBot.Models
{
    public class PrivatBankRatesModel
    {
        public string date { get; set; }
        public string bank { get; set; }
        public int baseCurrency { get; set; }
        public string baseCurrencyLit { get; set; }
        public List<PrivatBankRateModel> exchangeRate { get; set; }
    }
}
