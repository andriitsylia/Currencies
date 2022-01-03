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

        public async Task<RatesServiceModel> Get(DateTime date)
        {
            string jsonData = await GetPerDateAsJson(date);
            JsonElement root = JsonDocument.Parse(jsonData).RootElement;
            RatesServiceModel rates; 
            switch (_bank.Name)
            {
                case "NBU":
                    NBURatesSourceModel nbuRates = new(JsonSerializer.Deserialize<List<NBURateSourceModel>>(root.ToString()));
                    rates = Parse(nbuRates, date);
                    break;

                case "Privatbank":
                    PrivatBankRatesSourceModel privatBankRates = JsonSerializer.Deserialize<PrivatBankRatesSourceModel>(root.ToString());
                    rates = Parse(privatBankRates, date);
                    break;
                case "Monobank":
                    throw new NotImplementedException();
                    break;

                default:
                    throw new NotImplementedException();
                    break;
            }
            return rates;
        }

        private RatesServiceModel Parse(NBURatesSourceModel nbuRates, DateTime date)
        {
            RatesServiceModel rates = new(_bank.Name, date);
            Currency baseCurrency = Currency.UAH;

            foreach (var rate in nbuRates.Rates)
            {
                if (Enum.TryParse(typeof(Currency), rate.CurrencyCodeL, true, out object currency))
                {
                    rates.Rates.Add(new RateServiceModel((Currency)currency, baseCurrency, rate.Amount/rate.Units, rate.Amount / rate.Units));
                }
            }
            return rates;
        }

        private RatesServiceModel Parse(PrivatBankRatesSourceModel privatBankRates, DateTime date)
        {
            RatesServiceModel rates = new(_bank.Name, date);
            foreach (var rate in privatBankRates.exchangeRate)
            {
                if (Enum.TryParse(typeof(Currency), rate.currency, true, out object currency) &&
                    Enum.TryParse(typeof(Currency), rate.baseCurrency, true, out object baseCurrency))
                {
                    rates.Rates.Add(new RateServiceModel((Currency)currency, (Currency)baseCurrency, rate.purchaseRate, rate.saleRate));
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
