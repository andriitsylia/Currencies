using System;
using System.Collections.Generic;

namespace TelegramBot.Models
{
    public class NBURatesModel
    {
        public List<NBURateModel> Rates { get; set; }

        public NBURatesModel(IEnumerable<NBURateModel> rates)
        {
            Rates = (List<NBURateModel>)rates ?? throw new ArgumentNullException(nameof(rates), "Received a null argument");
        }
    }
}
