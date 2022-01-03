using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class RatesServiceModel
    {
        public string Bank { get; set; }
        public DateTime Date { get; set; }
        public List<RateServiceModel> Rates { get; set; }

        public RatesServiceModel(string bank, DateTime date)
        {
            Bank = bank;
            Date = date;
            Rates = new List<RateServiceModel>();
        }

        public RateServiceModel GetRate(string currency)
        {
            return Rates.Find(c => c.Currency.ToString().ToUpper() == currency.ToUpper());
        }

        public RateServiceModel GetRate(Currency currenncy)
        {
            return Rates.Find(c => c.Currency == currenncy);
        }
    }
}
