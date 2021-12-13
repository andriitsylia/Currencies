using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Models;

namespace TelegramBot.Services
{
    public class GetJsonDataFromBank
    {
        private const string PRIVAT_BANK_CONNECTION_ADRESS = "https://api.privatbank.ua/p24api/exchange_rates?json&date=";
        private const string DATE_FORMAT = "dd.MM.yyyy";

        private BankSettings _bank;
        private string _connectionAddress;
        private HttpClient _client;

        public GetJsonDataFromBank(BankSettings bank, string connectionAddress)
        {
            _bank = bank;
            _connectionAddress = connectionAddress ?? throw new ArgumentNullException(nameof(connectionAddress), "Received a null argument");
            _client = new HttpClient();
        }

        public async Task<string> Get(DateTime date)
        {
            _client.BaseAddress = new Uri(_connectionAddress + date.Date.ToString(DATE_FORMAT));
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
