using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Models;

namespace TelegramBot.Services
{
    public class JsonCurrencyRatesFromBank
    {
        private const string PRIVAT_BANK_EXCHANGE_RATE_PER_DATE = "https://api.privatbank.ua/p24api/exchange_rates?json&date=";
        private const string DATE_FORMAT = "dd.MM.yyyy";

        private Bank _bank;
        private string _connection;
        private HttpClient _client;

        public JsonCurrencyRatesFromBank(Bank bank, string connection)
        {
            _bank = bank;
            _connection = connection ?? throw new ArgumentNullException(nameof(connection), "Received a null argument");
            _client = new HttpClient();
        }

        public async Task<string> Get(DateTime date)
        {
            _client.BaseAddress = new Uri(_connection + date.Date.ToString(DATE_FORMAT));
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
