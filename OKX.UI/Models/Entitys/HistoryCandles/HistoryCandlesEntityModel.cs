using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models.Entitys.HistoryCandles
{
    public class HistoryCandlesEntityModel
    {
        public string code { get; set; }
        public string msg { get; set; }
        public string[][] data { get; set; }
    }
}
