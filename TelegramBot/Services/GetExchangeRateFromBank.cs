using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace TelegramBot.Services
{
    public class GetExchangeRateFromBank
    {
        private const string PRIVAT_BANK_EXCHANGE_CASH_RATE = "https://api.privatbank.ua/p24api/pubinfo?json&exchange&coursid=5";
        private const string PRIVAT_BANK_EXCHANGE_CASHLESS_RATE = "https://api.privatbank.ua/p24api/pubinfo?exchange&json&coursid=11";
        private const string PRIVAT_BANK_EXCHANGE_RATE_PER_DATE = "https://api.privatbank.ua/p24api/exchange_rates?json&date=";
        private const string DATE_FORMAT = "dd.MM.yyyy";

        private string _bank;
        private HttpClient _client;

        public GetExchangeRateFromBank(string bank)
        {
            _bank = bank ?? "PrivatBank";
            _client = new HttpClient();
        }

        public async Task<string> GetCashAsJson()
        {
            _client.BaseAddress = new Uri(PRIVAT_BANK_EXCHANGE_CASH_RATE);
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

        public async Task<string> GetCashlessAsJson()
        {
            _client.BaseAddress = new Uri(PRIVAT_BANK_EXCHANGE_CASHLESS_RATE);
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

        public async Task<string> GetPerDateAsJson(DateTime date)
        {
            _client.BaseAddress = new Uri(PRIVAT_BANK_EXCHANGE_RATE_PER_DATE + date.Date.ToString(DATE_FORMAT));
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
