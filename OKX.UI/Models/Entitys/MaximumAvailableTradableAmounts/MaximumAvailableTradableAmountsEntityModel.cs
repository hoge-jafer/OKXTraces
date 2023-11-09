using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models.Entitys.MaximumAvailableTradableAmounts
{
    public class MaximumAvailableTradableAmountsEntityModel
    {
        public string code { get; set; }
        public MaximumAvailableTradableAmountsDatumEntityModel[] data { get; set; }
        public string msg { get; set; }
    }
}
