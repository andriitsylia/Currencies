using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Settings
{
    public class BanksSettings
    {

        public Banks Get()
        {
            Banks banks = new();
            IConfiguration mainSettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            banks.Items = mainSettings.GetSection("Bank").Get<IEnumerable<Bank>>();

            return banks;
        }
    }
}
