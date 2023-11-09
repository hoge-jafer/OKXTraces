using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models.Entitys.MaximumAvailableTradableAmounts
{
    public class MaximumAvailableTradableAmountsDatumEntityModel
    {
        public string ccy { get; set; }
        public string instId { get; set; }
        public string maxBuy { get; set; }
        public string maxSell { get; set; }
    }
}
