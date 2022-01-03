using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class CurrencyListServiceModel
    {
        private readonly string _bank;
        private readonly List<string> _currencies;
        private readonly string _date;

        public List<string> Currencies
        {
            get => _currencies;
        }

        public CurrencyListServiceModel(PrivatBankRatesSourceModel currencyRates)
        {
            _bank = currencyRates.bank;
            _date = currencyRates.date;
            _currencies = currencyRates.exchangeRate.Select(c => c.currency).ToList<string>();
        }

        public string GetCurrency(string currency)
        {
            return _currencies.Find(c => !string.IsNullOrWhiteSpace(c) && c.ToUpper() == currency.ToUpper());
        }
    }
}
