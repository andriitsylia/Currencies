using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class CurrencyListModel
    {
        public string Bank { get; }
        public List<Currency> Currencies { get; }
        public string Date;

        public CurrencyListModel(RatesModel rates)
        {
            Bank = rates.Bank;
            Date = rates.Date.ToString();
            Currencies = rates.Rates.OrderBy(o => o.Currency.ToString()).Select(c => c.Currency).ToList();
        }

        public string GetCurrency(string currency)
        {
            return Currencies.Find(c => c.ToString().ToUpper() == currency.ToUpper()).ToString();
        }
    }
}
