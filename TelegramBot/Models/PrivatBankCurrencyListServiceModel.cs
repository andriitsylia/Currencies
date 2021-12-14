using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class PrivatBankCurrencyListServiceModel
    {
        private readonly string _bank;
        private readonly List<string> _currencies;
        private readonly string _date;

        public List<string> Currencies
        {
            get => _currencies;
        }

        public PrivatBankCurrencyListServiceModel(PrivatBankRatesSourceModel currencyRates)
        {
            _bank = currencyRates.bank;
            _date = currencyRates.date;
            //var c = currencyRates.exchangeRate.Select(c => c.currency);
            _currencies = currencyRates.exchangeRate.Select(c => c.currency).ToList<string>();//new List<string>(c);
        }
    }
}
