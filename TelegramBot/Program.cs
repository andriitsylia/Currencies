using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Models;
using TelegramBot.Services;
using TelegramBot.Settings;

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
