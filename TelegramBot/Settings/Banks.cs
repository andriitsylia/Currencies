using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Settings
{
    public class Banks
    {
        public IEnumerable<Bank> Items { get; set; }

        public Bank GetBank(string bankName)
        {
            return Items.FirstOrDefault(b => b.Name.ToUpper() == bankName.ToUpper());
        }

        public override string ToString()
        {
            return String.Join("\n", Items.Select(i => i.Name));
        }
    }
}
