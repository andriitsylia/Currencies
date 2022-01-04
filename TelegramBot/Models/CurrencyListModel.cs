using System;
using System.Collections.Generic;
using System.Linq;

namespace TelegramBot.Models
{
    public class CurrencyListModel
    {
        public List<Currency> Currencies { get; }

        public CurrencyListModel(RatesModel rates)
        {
            Currencies = rates.Rates.OrderBy(o => o.Currency.ToString()).Select(c => c.Currency).ToList();
        }

        public Currency Get(string currency)
        {
            return Currencies.Find(c => c.ToString().ToUpper() == currency.ToUpper());
        }

        public bool IsValid(string currency)
        {
            return Enum.IsDefined(typeof(Currency), currency.ToUpper());
        }
    }
}
