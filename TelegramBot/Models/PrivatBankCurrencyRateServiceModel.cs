using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class PrivatBankCurrencyRateServiceModel
    {
        private readonly PrivatBankRatesSourceModel _currencyRates;
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

        public PrivatBankCurrencyRateServiceModel(PrivatBankRatesSourceModel currencyRates, string currency)
        {
            _currencyRates = currencyRates ?? throw new ArgumentNullException(nameof(currencyRates), "Received a null argument");
            _date = currencyRates.date;
            _baseCurrency = currencyRates.baseCurrencyLit;
            _currency = currency;
            _purchaseRate = _currencyRates.exchangeRate.FirstOrDefault(c => c.currency == _currency).purchaseRate;
            _saleRate = _currencyRates.exchangeRate.FirstOrDefault(c => c.currency == _currency).saleRate;
        }
    }
}
