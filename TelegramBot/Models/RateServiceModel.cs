using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class RateServiceModel
    {
        public Currency Currency { get; set; }
        public Currency BaseCurrency { get; set; }
        public float Purchase { get; set; }
        public float Sale { get; set; }

        public RateServiceModel(Currency currency, Currency baseCurrency, float purchase, float sale)
        {
            Currency = currency;
            BaseCurrency = baseCurrency;
            Purchase = purchase;
            Sale = sale;
        }
    }
}
