using System;

namespace TelegramBot.Models
{
    public class ReportModel
    {
        public RateModel Rate { get; }

        public string Report { get; }

        public ReportModel(RateModel rate)
        {
            Rate = rate ?? throw new ArgumentNullException(nameof(rate), "Received a null argument");
            Report = Make();
        }

        private string Make()
        {
            return $"Курс {Rate.Currency} у відношенні до {Rate.BaseCurrency} cтановить:\n" +
                   $"Купівля: {Rate.Purchase:F4}\n" +
                   $"Продаж: {Rate.Sale:F4}\n";
        }
    }
}
