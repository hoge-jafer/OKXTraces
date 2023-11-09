using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models
{

    public class PlaceOrderModel
    {
        public string code { get; set; }
        public PlaceOrderModelDatumModel [] data { get; set; }
        public string msg { get; set; }
    }
}
