using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models
{
    public class MarketIndexTickersModel
    {
        public string code { get; set; }
        public string msg { get; set; }
        public MarketIndexTickersDatumModel[] data { get; set; }
    }
}
