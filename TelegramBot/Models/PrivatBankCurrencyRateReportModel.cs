﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class PrivatBankCurrencyRateReportModel
    {
        private readonly string _report;
        private readonly PrivatBankCurrencyRateServiceModel _currencyRate;

        public string Report
        {
            get => _report;
        }

        public PrivatBankCurrencyRateReportModel(PrivatBankCurrencyRateServiceModel currencyRate)
        {
            _currencyRate = currencyRate ?? throw new ArgumentNullException(nameof(currencyRate), "Received a null argument");
            _report = Create();
        }

        private string Create()
        {
            StringBuilder result = new StringBuilder();
            result.Append($"Курс {_currencyRate.Currency} у відношенні до {_currencyRate.BaseCurrency} cтаном на {_currencyRate.Date} року\r\n");
            result.Append($"Купівля: {_currencyRate.PurchaseRate:C7}\r\nПродаж: {_currencyRate.SaleRate:C7}\r\n");
            return result.ToString();
        }
    }
}
