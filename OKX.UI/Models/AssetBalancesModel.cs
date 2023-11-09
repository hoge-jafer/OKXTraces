using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models
{
    public class AssetBalancesModel
    {
        public string code { get; set; }
        public AssetBalancesDatumModel[] data { get; set; }
        public string msg { get; set; }
    }
}
