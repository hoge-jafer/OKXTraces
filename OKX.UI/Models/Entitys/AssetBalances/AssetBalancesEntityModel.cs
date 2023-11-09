using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models.Entitys.AssetBalances
{
  public  class AssetBalancesEntityModel
    {
        /// <summary>
        /// 可用余额
        /// </summary>
        public string availBal { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public string bal { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string ccy { get; set; }

        /// <summary>
        /// 冻结（不可用）
        /// </summary>
        public string frozenBal { get; set; }
    }
}
