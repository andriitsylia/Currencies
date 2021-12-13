using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Settings
{
    public class Bank
    {
        public string Name { get; set; }
        public string ConnectionAddress { get; set; }
        public string DateFormat { get; set; }
        public string DecimalDigits { get; set; }
    }
}
