using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OKX.UI.Models.Entitys.AccountBalance
{
    public class AccountBalanceEntityModel : Screen
    {
        /// <summary>
        /// 币的图片
        /// </summary>
        public ImageSource coinPath { get; set; }

        /// <summary>
        /// 可用余额 适用于简单交易模式
        /// </summary>
        public string availBal { get; set; }

        /// <summary>
        /// 可用保证金
        /// </summary>
        public string availEq { get; set; }

        /// <summary>
        /// 币种余额
        /// </summary>
        public string cashBal { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string ccy { get; set; }

        /// <summary>
        /// 币种全仓负债额
        /// </summary>
        public string crossLiab { get; set; }

        /// <summary>
        /// 美金层面币种折算权益
        /// </summary>
        public string disEq { get; set; }

        /// <summary>
        /// 币种总权益
        /// </summary>
        public string eq { get; set; }

        /// <summary>
        /// 币种权益美金价值
        /// </summary>
        public string eqUsd { get; set; }

        /// <summary>
        /// 币种占用金额
        /// </summary>
        public string frozenBal { get; set; }

        /// <summary>
        /// 计息
        /// </summary>
        public string interest { get; set; }

        /// <summary>
        /// 币种逐仓仓位权益
        /// </summary>
        public string isoEq { get; set; }

        /// <summary>
        /// 币种逐仓负债额
        /// </summary>
        public string isoLiab { get; set; }

        /// <summary>
        /// 逐仓未实现盈亏
        /// </summary>
        public string isoUpl { get; set; }

        /// <summary>
        /// 币种负债额
        /// </summary>
        public string liab { get; set; }

        /// <summary>
        /// 币种最大可借
        /// </summary>
        public string maxLoan { get; set; }

        /// <summary>
        /// 保证金率
        /// </summary>
        public string mgnRatio { get; set; }

        /// <summary>
        /// 币种杠杆倍数
        /// </summary>
        public string notionalLever { get; set; }

        /// <summary>
        /// 挂单冻结数量
        /// </summary>
        public string ordFrozen { get; set; }

        /// <summary>
        /// 策略权益
        /// </summary>
        public string stgyEq { get; set; }

        /// <summary>
        /// 当前负债币种触发系统自动换币的风险0、1、2、3、4、5其中之一，数字越大代表您的负债币种触发自动换币概率越高
        /// </summary>
        public string twap { get; set; }

        /// <summary>
        /// 币种余额信息的更新时间，Unix时间戳的毫秒数格式
        /// </summary>
        public string uTime { get; set; }

        /// <summary>
        /// 未实现盈亏
        /// </summary>
        public string upl { get; set; }

        /// <summary>
        /// 由于仓位未实现亏损导致的负债
        /// </summary>
        public string uplLiab { get; set; }
    }
}
