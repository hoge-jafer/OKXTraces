using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models
{
    public class AccountBalanceDatumModel
    {
        /// <summary>
        /// 美金层面有效保证金
        /// </summary>
        public string adjEq { get; set; }

        /// <summary>
        /// 各币种资产详细信息
        /// </summary>
        public AccountBalanceDetailModel[] details { get; set; }

        /// <summary>
        /// 美金层面占用保证金
        /// </summary>
        public string imr { get; set; }

        /// <summary>
        /// 美金层面逐仓仓位权益
        /// </summary>
        public string isoEq { get; set; }

        /// <summary>
        /// 美金层面保证金率
        /// </summary>
        public string mgnRatio { get; set; }

        /// <summary>
        /// 美金层面维持保证金
        /// </summary>
        public string mmr { get; set; }

        /// <summary>
        /// 以美金价值为单位的持仓数量，即仓位美金价值
        /// </summary>
        public string notionalUsd { get; set; }

        /// <summary>
        /// 美金层面全仓挂单占用保证金
        /// </summary>
        public string ordFroz { get; set; }

        /// <summary>
        /// 美金层面权益
        /// </summary>
        public string totalEq { get; set; }

        /// <summary>
        /// 账户信息的更新时间，Unix时间戳的毫秒数格式，如 1597026383085
        /// </summary>
        public string uTime { get; set; }
    }
}
