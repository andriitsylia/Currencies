using System;
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

        public async Task<PrivatBankRatesSourceModel> Get(DateTime date)
        {
            string jsonData = await GetPerDateAsJson(date);
            JsonElement root = JsonDocument.Parse(jsonData).RootElement;

            return JsonSerializer.Deserialize<PrivatBankRatesSourceModel>(root.ToString());
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
