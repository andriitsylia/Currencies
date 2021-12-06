using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Models;

namespace TelegramBot.Services
{
    public class PBExchangeRateNow
    {

        public static PBExchangeRate GetFor(string currency, PBExchangeRateList currencyList)
        {
            return currencyList.Rates.Where(cur => cur.ccy == currency).Single();
        }
    }
}
