using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class ReportModel
    {
        private readonly string _report;
        private readonly CurrencyRateServiceModel _currencyRate;

        public string Report
        {
            get => _report;
        }

        public ReportModel(CurrencyRateServiceModel currencyRate)
        {
            _currencyRate = currencyRate ?? throw new ArgumentNullException(nameof(currencyRate), "Received a null argument");
            _report = Create();
        }

        private string Create()
        {
            StringBuilder result = new StringBuilder();
            result.Append($"Курс {_currencyRate.Currency} у відношенні до {_currencyRate.BaseCurrency} cтаном на {_currencyRate.Date} року\n");
            result.Append($"Купівля: {_currencyRate.PurchaseRate:F4}\nПродаж: {_currencyRate.SaleRate:F4}\n");
            return result.ToString();
        }
    }
}
