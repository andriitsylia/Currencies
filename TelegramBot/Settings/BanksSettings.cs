using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace TelegramBot.Settings
{
    public class BanksSettings
    {

        public Banks Get()
        {
            Banks banks = new();
            IConfiguration mainSettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            banks.Items = mainSettings.GetSection("Bank").Get<List<Bank>>();

            return banks;
        }
    }
}
