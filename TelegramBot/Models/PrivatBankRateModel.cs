using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class PrivatBankRateModel
    {
        public string baseCurrency { get; set; }
        public string currency { get; set; }
        public float saleRateNB { get; set; }
        public float purchaseRateNB { get; set; }
        public float saleRate { get; set; }
        public float purchaseRate { get; set; }
    }
}
