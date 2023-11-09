using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models
{
   public class MarketTickerModel
    {
        public string code { get; set; }
        public string msg { get; set; }
        public MarketTickerDatumModel[] data { get; set; }
    }

}
