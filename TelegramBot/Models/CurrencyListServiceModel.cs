using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class CurrencyListServiceModel
    {
        public string Bank { get; }
        public List<Currency> Currencies { get; }
        public string Date;

        public CurrencyListServiceModel(RatesServiceModel rates)
        {
            Bank = rates.Bank;
            Date = rates.Date.ToString();
            Currencies = rates.Rates.Select(c => c.Currency).ToList<Currency>();
        }

        public string GetCurrency(string currency)
        {
            return Currencies.Find(c => c.ToString().ToUpper() == currency.ToUpper()).ToString();
        }
    }
}
