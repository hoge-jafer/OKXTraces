using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Models.Entitys.MarketIndexTickers
{
   public class MarketIndexTickersEntityModel
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        public string instId { get; set; }

        /// <summary>
        /// 最新指数价格
        /// </summary>
        public string idxPx { get; set; }

        /// <summary>
        /// 24小时指数最高价格
        /// </summary>
        public string high24h { get; set; }

        /// <summary>
        /// UTC 0 时开盘价
        /// </summary>
        public string sodUtc0 { get; set; }

        /// <summary>
        /// 24小时开盘价
        /// </summary>
        public string open24h { get; set; }

        /// <summary>
        /// 24小时指数最低价格
        /// </summary>
        public string low24h { get; set; }

        /// <summary>
        /// UTC+8 时开盘价
        /// </summary>
        public string sodUtc8 { get; set; }

        /// <summary>
        /// 指数价格更新时间，Unix时间戳的毫秒数格式，如 1597026383085
        /// </summary>
        public string ts { get; set; }
    }
}
