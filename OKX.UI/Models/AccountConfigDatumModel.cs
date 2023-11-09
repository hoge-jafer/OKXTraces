using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models
{
    public class AccountConfigDatumModel
    {
        /// <summary>
        /// 账户层级1：简单交易模式，2：单币种保证金模式 ，3：跨币种保证金模式 ，4：组合保证金模式
        /// </summary>
        public string acctLv { get; set; }

        /// <summary>
        /// 是否自动借币
        /// </summary>
        public bool autoLoan { get; set; }

        /// <summary>
        /// 衍生品的逐仓保证金划转模式 automatic：开仓划转 autonomy：自主划转
        /// </summary>
        public string ctIsoMode { get; set; }

        /// <summary>
        /// 当前希腊字母展示方式 PA：币本位 BS：美元本位
        /// </summary>
        public string greeksType { get; set; }

        /// <summary>
        /// 当前在平台上真实交易量的用户等级
        /// </summary>
        public string level { get; set; }

        /// <summary>
        /// 特约用户的临时体验用户等级
        /// </summary>
        public string levelTmp { get; set; }

        /// <summary>
        /// 币币杠杆的逐仓保证金划转模式 automatic：开仓划转 autonomy：自主划转
        /// </summary>
        public string mgnIsoMode { get; set; }

        /// <summary>
        /// 持仓方式  long_short_mode：双向持仓 net_mode：单向持仓仅适用交割/永续
        /// </summary>
        public string posMode { get; set; }

        /// <summary>
        /// 账户ID，账户uid和app上的一致
        /// </summary>
        public string uid { get; set; }
    }
}
