using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using TelegramBot.Services;

namespace TelegramBot
{
    public class Program
    {
        public static async Task Main()
        {
            IConfiguration  mainSettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            string token = mainSettings.GetSection("TelegramToken").Value;

            Bot bot = new(token);
            await Bot.Run();
        }
    }
}
