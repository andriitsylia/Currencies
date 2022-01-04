namespace TelegramBot.Models
{
    public class RateModel
    {
        public Currency Currency { get; set; }
        public Currency BaseCurrency { get; set; }
        public float Purchase { get; set; }
        public float Sale { get; set; }

        public RateModel(Currency currency, Currency baseCurrency, float purchase, float sale)
        {
            Currency = currency;
            BaseCurrency = baseCurrency;
            Purchase = purchase;
            Sale = sale;
        }
    }
}
