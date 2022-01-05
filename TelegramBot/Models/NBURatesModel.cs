using System;
using System.Collections.Generic;
using TelegramBot.Constants;

namespace TelegramBot.Models
{
    public class NBURatesModel
    {
        public List<NBURateModel> Rates { get; set; }

        public NBURatesModel(IEnumerable<NBURateModel> rates)
        {
            Rates = (List<NBURateModel>)rates ?? throw new ArgumentNullException(nameof(rates), BotInfoMessage.NULL_ARGUMENT);
        }
    }
}
