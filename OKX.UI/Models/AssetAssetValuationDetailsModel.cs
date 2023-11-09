using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models
{
    public class AssetAssetValuationDetailsModel
    {
        /// <summary>
        /// 经典账户
        /// </summary>
        public string classic { get; set; }

        /// <summary>
        /// 金融账户
        /// </summary>
        public string earn { get; set; }

        /// <summary>
        /// 资金账户
        /// </summary>
        public string funding { get; set; }

        /// <summary>
        /// 交易账户
        /// </summary>
        public string trading { get; set; }
    }
}
