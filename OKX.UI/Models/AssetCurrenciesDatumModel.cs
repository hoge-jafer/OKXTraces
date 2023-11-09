using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models
{
    public class AssetCurrenciesDatumModel
    {
        /// <summary>
        /// 是否可充值，false表示不可链上充值，true表示可以链上充值
        /// </summary>
        public bool canDep { get; set; }

        /// <summary>
        /// 是否可内部转账，false表示不可内部转账，true表示可以内部转账
        /// </summary>
        public bool canInternal { get; set; }

        /// <summary>
        /// 是否可提币，false表示不可链上提币，true表示可以链上提币
        /// </summary>
        public bool canWd { get; set; }

        /// <summary>
        /// 币种名称
        /// </summary>
        public string ccy { get; set; }

        /// <summary>
        /// 币种链信息有的币种下有多个链，必须要做区分，如USDT下有USDT-ERC20，USDT-TRC20，USDT-Omni多个链
        /// </summary>
        public string chain { get; set; }

        /// <summary>
        /// 当前链是否为主链如果是则返回true，否则返回false
        /// </summary>
        public bool mainNet { get; set; }

        /// <summary>
        /// 最大提币手续费数量
        /// </summary>
        public string maxFee { get; set; }

        /// <summary>
        /// 最小提币手续费数量
        /// </summary>
        public string minFee { get; set; }

        /// <summary>
        /// 币种最小提币量
        /// </summary>
        public string minWd { get; set; }

        /// <summary>
        /// 币种中文名称，不显示则无对应名称
        /// </summary>
        public string name { get; set; }
    }
}
