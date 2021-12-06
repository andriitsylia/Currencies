using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Models;

namespace TelegramBot.Services
{
    public class PBReportMessage
    {
        public string Rate(PBExchangeRate rate)
        {
            return $"Поточний курс {rate.ccy} у відношенні до {rate.base_ccy}\r\nКупівля: {rate.buy}\r\nПродаж: {rate.sale}";
        }

        public string Rate(PBExchangeRatePerDate rate, DateTime date)
        {
            StringBuilder result = new StringBuilder();
            result.Append($"Курс {rate.baseCurrency} у відношенні до {rate.currency} cтаном на {date.ToString("dd.MM.yyyy")} року\r\n");
            result.Append($"Купівля (НБУ): {rate.purchaseRateNB}\r\nПродаж (НБУ): {rate.saleRateNB}\r\n");
            result.Append($"Купівля (Приватбанк): {rate.purchaseRate}\r\nПродаж (Приватбанк): {rate.saleRate}");
            return result.ToString();
        }

        public string CurrencyList(List<string> list)
        {
            return string.Join(" ", list);

        }
    }
}
