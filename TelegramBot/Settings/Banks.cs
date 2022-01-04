﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TelegramBot.Settings
{
    public class Banks
    {
        public List<Bank> Items { get; set; }

        public Bank GetBank(string bankName)
        {
            if (string.IsNullOrWhiteSpace(bankName))
            {
                throw new ArgumentNullException(nameof(bankName), "Received a null argument");
            }

            return Items.Find(b => b.Name.ToUpper() == bankName.ToUpper());
        }
    }
}
