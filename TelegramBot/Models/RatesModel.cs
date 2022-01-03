using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class RatesModel
    {
        public string Bank { get; set; }
        public DateTime Date { get; set; }
        public List<RateModel> Rates { get; set; }

        public RatesModel(string bank, DateTime date)
        {
            Bank = bank;
            Date = date;
            Rates = new List<RateModel>();
        }

        public RateModel GetRate(string currency)
        {
            return Rates.Find(c => c.Currency.ToString().ToUpper() == currency.ToUpper());
        }

        public RateModel GetRate(Currency currenncy)
        {
            return Rates.Find(c => c.Currency == currenncy);
        }
    }
}
