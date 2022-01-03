using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TelegramBot.Models;
using TelegramBot.Settings;

namespace TelegramBot.Services
{
    public class BankRates
    {
        private HttpClient _client;
        private Bank _bank;

        public BankRates(Bank bank)
        {
            _bank = bank ?? throw new ArgumentNullException(nameof(bank), "Received a null argument");
            _client = new HttpClient();
        }

        public async Task<RatesModel> Get(DateTime date)
        {
            string jsonData = await GetPerDateAsJson(date);
            JsonElement root = JsonDocument.Parse(jsonData).RootElement;
            RatesModel rates; 
            switch (_bank.Name)
            {
                case "NBU":
                    NBURatesModel nbuRates = new(JsonSerializer.Deserialize<List<NBURateModel>>(root.ToString()));
                    rates = Parse(nbuRates, date);
                    break;

                case "Privatbank":
                default:
                    PrivatBankRatesModel privatBankRates = JsonSerializer.Deserialize<PrivatBankRatesModel>(root.ToString());
                    rates = Parse(privatBankRates, date);
                    break;
            }
            return rates;
        }

        private RatesModel Parse(NBURatesModel nbuRates, DateTime date)
        {
            RatesModel rates = new(_bank.Name, date);
            Currency baseCurrency = Currency.UAH;

            foreach (var rate in nbuRates.Rates)
            {
                if (Enum.TryParse(typeof(Currency), rate.CurrencyCodeL, true, out object currency))
                {
                    rates.Rates.Add(new RateModel((Currency)currency, baseCurrency, rate.Amount/rate.Units, rate.Amount / rate.Units));
                }
            }
            return rates;
        }

        private RatesModel Parse(PrivatBankRatesModel privatBankRates, DateTime date)
        {
            RatesModel rates = new(_bank.Name, date);
            foreach (var rate in privatBankRates.exchangeRate)
            {
                if (Enum.TryParse(typeof(Currency), rate.currency, true, out object currency) &&
                    Enum.TryParse(typeof(Currency), rate.baseCurrency, true, out object baseCurrency))
                {
                    rates.Rates.Add(new RateModel((Currency)currency, (Currency)baseCurrency, rate.purchaseRate, rate.saleRate));
                }
            }
            return rates;
        }

        public async Task<string> GetPerDateAsJson(DateTime date)
        {
            _client.BaseAddress = new Uri(_bank.ConnectionAddress + date.Date.ToString(_bank.DateFormat));
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage httpResponseMessage = await _client.GetAsync(_client.BaseAddress);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return await httpResponseMessage.Content.ReadAsStringAsync();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
