using System.Collections.Generic;

namespace TelegramBot.Models
{
    public class PrivatBankRatesSourceModel
    {
        public string date { get; set; }
        public string bank { get; set; }
        public int baseCurrency { get; set; }
        public string baseCurrencyLit { get; set; }
        public List<PrivatBankRateSourceModel> exchangeRate { get; set; }
    }
}
