using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models
{
    public class MarketTickerDatumModel
    {
        /// <summary>
        /// 产品类型
        /// </summary>
        public string instType { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string instId { get; set; }

        /// <summary>
        /// 最新成交价
        /// </summary>
        public string last { get; set; }

        /// <summary>
        /// 最新成交的数量
        /// </summary>
        public string lastSz { get; set; }

        /// <summary>
        /// 卖一价
        /// </summary>
        public string askPx { get; set; }

        /// <summary>
        /// 卖一价的挂单数数量
        /// </summary>
        public string askSz { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string bidPx { get; set; }

        /// <summary>
        /// 买一价
        /// </summary>
        public string bidSz { get; set; }

        /// <summary>
        /// 买一价的挂单数量
        /// </summary>
        public string open24h { get; set; }

        /// <summary>
        /// 24小时最高价
        /// </summary>
        public string high24h { get; set; }

        /// <summary>
        /// 24小时最低价
        /// </summary>
        public string low24h { get; set; }

        /// <summary>
        /// 24小时成交量，以币为单位如果是衍生品合约，数值为交易货币的数量。如果是币币/币币杠杆，数值为计价货币的数量
        /// </summary>
        public string volCcy24h { get; set; }

        /// <summary>
        /// 24小时成交量，以张为单位 如果是衍生品合约，数值为合约的张数。如果是币币/币币杠杆，数值为交易货币的数量。
        /// </summary>
        public string vol24h { get; set; }

        /// <summary>
        /// ticker数据产生时间，Unix时间戳的毫秒数格式，如 1597026383085
        /// </summary>
        public string ts { get; set; }

        /// <summary>
        /// UTC 0 时开盘价
        /// </summary>
        public string sodUtc0 { get; set; }

        /// <summary>
        /// UTC+8 时开盘价
        /// </summary>
        public string sodUtc8 { get; set; }
    }
}
