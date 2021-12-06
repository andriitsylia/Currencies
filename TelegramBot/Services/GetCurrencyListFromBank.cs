using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TelegramBot.Models;

namespace TelegramBot.Services
{
    public class GetCurrencyListFromBank
    {
        private Bank _bank;
        private List<string> _currency;

        public List<string> Currency
        {
            get => _currency;
            set => _currency = value;
        }

        public GetCurrencyListFromBank(Bank bank)
        {
            _bank = bank;
            _currency = Get();
        }

        private List<string> Get()
        {
            GetExchangeRateFromBank rate = new(_bank.ToString());
            Task<string> jsonRate = rate.GetPerDateAsJson(DateTime.Now);
            
            using JsonDocument doc = JsonDocument.Parse(jsonRate.Result);
            JsonElement root = doc.RootElement;
            List<string> result = new();

            switch (_bank.ToString())
            {
                case "PrivatBank":
                    PBExchangeRatePerDateList rateList = JsonSerializer.Deserialize<PBExchangeRatePerDateList>(root.ToString());
                    foreach (PBExchangeRatePerDate item in rateList.exchangeRate)
                    {
                        result.Add(item.currency);
                    }
                    break;
            }

            return result;
        }
    
            }
}
