using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class NBURatesSourceModel
    {
        public List<NBURateSourceModel> Rates { get; set; }
        public NBURatesSourceModel(IEnumerable<NBURateSourceModel> rates)
        {
            Rates = (List<NBURateSourceModel>)rates ?? throw new ArgumentNullException(nameof(rates), "Received a null argument");
        }
    }
}
