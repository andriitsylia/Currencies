using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TelegramBot.Models;
using TelegramBot.Settings;

namespace TelegramBot.Services
{
    public class JsonRatesParse
    {
        public static PrivatBankRatesSourceModel Parse(Bank bank, DateTime date)
        {
            string jsonData = new BankCurrencyRates(bank).GetPerDateAsJson(date).Result;
            JsonElement root = JsonDocument.Parse(jsonData).RootElement;

            return JsonSerializer.Deserialize<PrivatBankRatesSourceModel>(root.ToString());
        }
    }
}
