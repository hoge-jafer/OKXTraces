using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models.Entitys.MaximumAvailableTradableAmount
{
    public class MaximumAvailableTradableAmountEntityModel
    {
        public string code { get; set; }
        public MaximumAvailableTradableAmountDatumEntityModel[] data { get; set; }
        public string msg { get; set; }
    }
}
