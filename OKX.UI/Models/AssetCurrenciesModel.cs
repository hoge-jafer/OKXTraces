using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models
{
    public class AssetCurrenciesModel
    {
        public string code { get; set; }
        public AssetCurrenciesDatumModel[] data { get; set; }
        public string msg { get; set; }
    }
}
