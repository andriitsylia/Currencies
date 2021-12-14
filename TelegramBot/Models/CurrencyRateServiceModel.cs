using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class CurrencyRateServiceModel
    {
        private readonly string _baseCurrency;
        private readonly string _currency;
        private readonly float _purchaseRate;
        private readonly float _saleRate;
        private readonly string _date;

        public string BaseCurrency
        { 
            get => _baseCurrency;
        }

        public string Currency
        { 
            get => _currency;
        }

        public float PurchaseRate
        { 
            get => _purchaseRate;
        }

        public float SaleRate
        {
            get => _saleRate;
        }

        public string Date
        {
            get => _date;
        }

        public CurrencyRateServiceModel(PrivatBankRatesSourceModel currencyRates, string currency)
        {
            if (currencyRates == null)
            {
                throw new ArgumentNullException(nameof(currencyRates), "Received a null argument");
            }

            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentNullException(nameof(currency), "Received a null argument");
            }

            _date = currencyRates.date;
            _baseCurrency = currencyRates.baseCurrencyLit;
            _currency = currency;
            _purchaseRate = currencyRates.exchangeRate.FirstOrDefault(c => c.currency == _currency).purchaseRate;
            _saleRate = currencyRates.exchangeRate.FirstOrDefault(c => c.currency == _currency).saleRate;
        }
    }
}
