using Caliburn.Micro;
using Newtonsoft.Json;
using OKX.UI.Models;
using OKX.UI.Models.Entitys.AccountBalance;
using OKXSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OKX.UI.ViewModels
{
    public class AccountBalanceViewModel : Screen
    {
        private INavigationService navigationService;
        public AccountBalanceViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            GetAccountBalanceData();
        }

        public AccountBalanceViewModel()
        {
            GetAccountBalanceData();
        }

        private string apiKey = "";
        private string secret = "";
        private string passPhrase = "";

        #region

        private BindableCollection<AccountBalanceEntityModel> accountBalanceEntityModels = new BindableCollection<AccountBalanceEntityModel>();
        public BindableCollection<AccountBalanceEntityModel> AccountBalanceEntityModels
        {
            get { return accountBalanceEntityModels; }
            set { Set(ref accountBalanceEntityModels, value); }
        }

        #endregion


        public async void GetAccountBalanceData()
        {
            AccountApi accountApi = new AccountApi(apiKey, secret, passPhrase);
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls;
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            Task<string> task = accountApi.GetBalance(requestParams);
            string result = await task;
            AccountBalanceModel accountBalanceModel = JsonConvert.DeserializeObject<AccountBalanceModel>(result);
            Debug.WriteLine("" + accountBalanceModel.data.Length);
            for (int i = 0; i < accountBalanceModel.data[0].details.Length; i++)
            {
                AccountBalanceEntityModel accountBalanceEntityModel = new AccountBalanceEntityModel();
                /// <summary>
                /// 可用余额 适用于简单交易模式
                /// </summary>
                accountBalanceEntityModel.availBal = accountBalanceModel.data[0].details[i].availBal;
                //MessageBox.Show(accountBalanceEntityModel.availBal);
                /// <summary>
                /// 可用保证金
                /// </summary>
                accountBalanceEntityModel.availEq = accountBalanceModel.data[0].details[i].availEq;

                /// <summary>
                /// 币种余额
                /// </summary>
                accountBalanceEntityModel.cashBal = accountBalanceModel.data[0].details[i].cashBal;

                /// <summary>
                /// 币种
                /// </summary>
                accountBalanceEntityModel.ccy = accountBalanceModel.data[0].details[i].ccy;
                string path = Directory.GetCurrentDirectory() + @"\Image\black\" + accountBalanceEntityModel.ccy.ToLower() + ".png";
                if (File.Exists(path))
                {
                    accountBalanceEntityModel.coinPath = GetImageDisplay(path);
                }


                /// <summary>
                /// 币种全仓负债额
                /// </summary>
                accountBalanceEntityModel.crossLiab = accountBalanceModel.data[0].details[i].crossLiab;

                /// <summary>
                /// 美金层面币种折算权益
                /// </summary>
                accountBalanceEntityModel.disEq = accountBalanceModel.data[0].details[i].disEq;

                /// <summary>
                /// 币种总权益
                /// </summary>
                accountBalanceEntityModel.eq = accountBalanceModel.data[0].details[i].eq;

                /// <summary>
                /// 币种权益美金价值
                /// </summary>
                accountBalanceEntityModel.eqUsd = accountBalanceModel.data[0].details[i].eqUsd;

                /// <summary>
                /// 币种占用金额
                /// </summary>
                accountBalanceEntityModel.frozenBal = accountBalanceModel.data[0].details[i].frozenBal;

                /// <summary>
                /// 计息
                /// </summary>
                accountBalanceEntityModel.interest = accountBalanceModel.data[0].details[i].interest;

                /// <summary>
                /// 币种逐仓仓位权益
                /// </summary>
                accountBalanceEntityModel.isoEq = accountBalanceModel.data[0].details[i].isoEq;

                /// <summary>
                /// 币种逐仓负债额
                /// </summary>
                accountBalanceEntityModel.isoLiab = accountBalanceModel.data[0].details[i].isoLiab;

                /// <summary>
                /// 逐仓未实现盈亏
                /// </summary>
                accountBalanceEntityModel.isoUpl = accountBalanceModel.data[0].details[i].isoUpl;

                /// <summary>
                /// 币种负债额
                /// </summary>
                accountBalanceEntityModel.liab = accountBalanceModel.data[0].details[i].liab;

                /// <summary>
                /// 币种最大可借
                /// </summary>
                accountBalanceEntityModel.maxLoan = accountBalanceModel.data[0].details[i].maxLoan;

                /// <summary>
                /// 保证金率
                /// </summary>
                accountBalanceEntityModel.mgnRatio = accountBalanceModel.data[0].details[i].mgnRatio;

                /// <summary>
                /// 币种杠杆倍数
                /// </summary>
                accountBalanceEntityModel.notionalLever = accountBalanceModel.data[0].details[i].notionalLever;

                /// <summary>
                /// 挂单冻结数量
                /// </summary>
                accountBalanceEntityModel.ordFrozen = accountBalanceModel.data[0].details[i].ordFrozen;

                /// <summary>
                /// 策略权益
                /// </summary>
                accountBalanceEntityModel.stgyEq = accountBalanceModel.data[0].details[i].stgyEq;

                /// <summary>
                /// 当前负债币种触发系统自动换币的风险0、1、2、3、4、5其中之一，数字越大代表您的负债币种触发自动换币概率越高
                /// </summary>
                accountBalanceEntityModel.twap = accountBalanceModel.data[0].details[i].twap;

                /// <summary>
                /// 币种余额信息的更新时间，Unix时间戳的毫秒数格式
                /// </summary>
                accountBalanceEntityModel.uTime = accountBalanceModel.data[0].details[i].uTime;

                /// <summary>
                /// 未实现盈亏
                /// </summary>
                accountBalanceEntityModel.upl = accountBalanceModel.data[0].details[i].upl;

                /// <summary>
                /// 由于仓位未实现亏损导致的负债
                /// </summary>
                accountBalanceEntityModel.uplLiab = accountBalanceModel.data[0].details[i].uplLiab;

                AccountBalanceEntityModels.Add(accountBalanceEntityModel);
            }

        }

        public void RegisterFrame(Frame frame)
        {
            navigationService = new FrameAdapter(frame);
            //container.Instance(navigationService);
            navigationService.NavigateToViewModel(typeof(TradeOrderViewModel));
        }

        public ImageSource GetImageDisplay(string path)
        {
            Trace.WriteLine("OKXUI GetImageDisplay："+path);

            ImageSource imageSource = null;
            var bitmapImage = new Bitmap(path, true);
            imageSource = BitmapToBitmapImage(bitmapImage);
            return imageSource;
        }

        public BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
    }
}
