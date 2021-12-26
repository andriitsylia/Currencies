﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Settings
{
    public class Banks
    {
        public IEnumerable<Bank> Items { get; set; }

        public override string ToString()
        {
            return String.Join("\n", Items.Select(i => i.Name));
        }
    }
}
