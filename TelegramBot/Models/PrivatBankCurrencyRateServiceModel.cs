using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class PrivatBankCurrencyRateServiceModel
    {
        private readonly PrivatBankCurrencyRatesSourceModel _currencyRates;
        private readonly string _baseCurrency;
        private readonly string _currency;
        private readonly string _purchaseRate;
        private readonly string _saleRate;
        private readonly string _date;

        public string BaseCurrency
        { 
            get => _baseCurrency;
        }

        public string Currency
        { 
            get => _currency;
        }

        public string PurchaseRate
        { 
            get => _purchaseRate;
        }

        public string SaleRate
        {
            get => _saleRate;
        }

        public string Date
        {
            get => _date;
        }

        public PrivatBankCurrencyRateServiceModel(PrivatBankCurrencyRatesSourceModel currencyRates, Currency currency)
        {
            _currencyRates = currencyRates ?? throw new ArgumentNullException(nameof(currencyRates), "Received a null argument");
            _date = currencyRates.date;
            _baseCurrency = currencyRates.baseCurrencyLit;
            _currency = currency.ToString();
            (_purchaseRate, _saleRate) = Get();
        }

        private (string pRate, string sRate) Get()
        {
            PrivatBankCurrencyRateSourceModel currencyRate;
            currencyRate = _currencyRates.exchangeRate.Where(c => c.currency == _currency).Single();
            return (currencyRate.purchaseRate.ToString(), currencyRate.saleRate.ToString());
        }
    }
}
