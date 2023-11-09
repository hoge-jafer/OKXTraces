using Caliburn.Micro;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using OKX.UI.Helper;
using OKX.UI.Models;
using OKX.UI.Models.Entitys.AccountBalance;
using OKX.UI.Models.Entitys.MarketTicker;
using OKX.UI.Models.Entitys.PlaceOrder;
using OKXSDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace OKX.UI.ViewModels
{
    public class ShellViewModel : Screen
    {
        private readonly SimpleContainer container;
        private INavigationService navigationService;
        private INavigationService mininavigationService;
        int sendmailcount = -1;
        MarketDataApi marketDataApi;
        Window window;
        IniConfigHelper iniconfighelper;
        string iniconfigpath = System.AppDomain.CurrentDomain.BaseDirectory + "Config\\Config.ini";


        public static uint SND_ASYNC = 0x0001;
        public static uint SND_FILENAME = 0x00020000;
        [DllImport("winmm.dll")]
        public static extern uint mciSendString(string lpstrCommand, string lpstrReturnString, uint uReturnLength, uint hWndCallback);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command">MCI命令字符串</param>
        /// <param name="returnString">存放反馈信息的缓冲区</param>
        /// <param name="returnSize">缓冲区的长度</param>
        /// <param name="hwndCallback">回调窗口的句柄，一般为NULL 若成功则返回0，否则返回错误码。</param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string command, string returnString, int returnSize, IntPtr hwndCallback);

        public ShellViewModel(SimpleContainer container)
        {
            this.container = container;
            //WindowStates = WindowState.Maximized;
            WindowStates = WindowState.Normal;
            Title = "";


            MiniVisibility = Visibility.Visible;
            MaxVisibility = Visibility.Collapsed;

            PinState = "\uE840";
            TopMost = true;
            //TopMost = false;
            SecretVisibility = Visibility.Collapsed;

            string directorypath = System.AppDomain.CurrentDomain.BaseDirectory + "Config";
            if (!Directory.Exists(directorypath))
            {
                Directory.CreateDirectory(directorypath);
            }

            #region 配置文件
            iniconfighelper = new IniConfigHelper(iniconfigpath);
            if (!File.Exists(iniconfigpath))
            {
                iniconfighelper.ConfigWriteValue("Config", "BuyPrice", "0");
                iniconfighelper.ConfigWriteValue("Config", "ApiKey", "NULL");
                iniconfighelper.ConfigWriteValue("Config", "Secret", "NULL");
                iniconfighelper.ConfigWriteValue("Config", "PassPhrase", "NULL");
                //iniconfighelper.ConfigWriteValue("Gate", "Collective", "BTC,ETH,BNB,LTC,LINK,ATOM,ETC,POND,LINA,EOS,XLM,AVAX,BCH,UNI,SUSHI,AAVE,XMR,DASH,ZEC,IOTX,IOST,1INCH,ADA,SCRT,ZEN,OSMO");
                iniconfighelper.ConfigWriteValue("Config", "Collective", "BTC,ETH,BNB,LTC,LINK,ATOM,ETC,EOS,XLM,AVAX,BCH,UNI,SUSHI,AAVE,XMR,DASH,ZEC,IOST,1INCH,ADA,ZEN");
            }
            else
            {
                ApiKeyEntity = iniconfighelper.ConfigReadValue("Config", "ApiKey");
                SecretEntity = iniconfighelper.ConfigReadValue("Config", "Secret");
                PassPhraseEntity = iniconfighelper.ConfigReadValue("Config", "PassPhrase");
                Collective = iniconfighelper.ConfigReadValue("Config", "Collective");
                marketDataApi = new MarketDataApi(ApiKeyEntity, SecretEntity, PassPhraseEntity);
            }

            #endregion

        }

        #region
        private ManualResetEvent manualReset = new ManualResetEvent(true);
        //     //开始执行后台操作
        //_backgroundWorker.RunWorkerAsync();
        private BackgroundWorker backgroundWorker;
        //      manualReset.Reset();//暂停当前线程的工作，发信号给waitOne方法，阻塞
        //        manualReset.Set();//继续当前线程的工作，发信号给waitOne方法，继续运行Waitone后面的代码
        //    _backgroundWorker.CancelAsync();//结束后台线程

        private void InitializeBackgroundWorker()
        {
            #region
            if (backgroundWorker == null)
                backgroundWorker = new BackgroundWorker();
            //bool类型，指示BackgroundWorker是否可以报告进度更新。当该属性值为True时，将可以成功调用ReportProgress方法
            backgroundWorker.WorkerReportsProgress = true;
            //bool类型，指示BackgroundWorker是否支持异步取消操作。当该属性值为True是，将可以成功调用CancelAsync方法
            backgroundWorker.WorkerSupportsCancellation = true;
            //执行RunWorkerAsync方法后触发DoWork，将异步执行backgroundWorker_DoWork方法中的代码
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            //执行ReportProgress方法后触发ProgressChanged，将执行ProgressChanged方法中的代码
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(_backgroundWorker_ProgressChanged);
            //异步操作完成或取消时执行的操作，当调用DoWork事件执行完成时触发。 
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            #endregion
        }

        //backgroundWorker_DoWork完成后执行
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if ((bool)e.Result == true)
            //{

            //}
        }

        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        //backgroundWorker_DoWork是运行在非UI线程上的，因此该内部代码应该避免与UI界面交互，交互的逻辑应该放到
        //ProgressChanged和RunWorkerCompleted事件中
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //e.Result = true;
            while (true)
            {
                //执行查询()
                GetAccountBalance();
                MonitorBitcoin();
                MonitorEthereum();
                MonitorLTCcoin();
                MonitorOKBcoin();
                MonitorBNBcoin();
                Thread.Sleep(110);
            }

        }

        #endregion

        public void RegisterFrame(Frame frame)
        {
            navigationService = new FrameAdapter(frame);

            container.Instance(navigationService);
            //navigationService.NavigateToViewModel(typeof(AccountBalanceViewModel));
        }

        public void MiniRegisterFrame(Frame frame)
        {
            //navigationService = new FrameAdapter(frame);
            //container.Instance(navigationService);
            //navigationService = new FrameAdapter(frame);
            //container.Instance(navigationService);
            navigationService.NavigateToViewModel(typeof(MarketTickerViewModel));
        }

        #region Model

        private WindowState windowStates;
        public WindowState WindowStates
        {
            get { return windowStates; }
            set { Set(ref windowStates, value); }
        }

        private string totalEq;
        public string TotalEq
        {
            get { return totalEq; }
            set { Set(ref totalEq, value); }
        }

        private string persent;
        public string Persent
        {
            get { return persent; }
            set { Set(ref persent, value); }
        }

        private string btcPrice;
        public string BTCPrice
        {
            get { return btcPrice; }
            set { Set(ref btcPrice, value); }
        }

        private string ethPersent;
        public string ETHPersent
        {
            get { return ethPersent; }
            set { Set(ref ethPersent, value); }
        }

        private string ethPrice;
        public string ETHPrice
        {
            get { return ethPrice; }
            set { Set(ref ethPrice, value); }
        }

        private string ltcPersent;
        public string LTCPersent
        {
            get { return ltcPersent; }
            set { Set(ref ltcPersent, value); }
        }

        private string ltcPrice;
        public string LTCPrice
        {
            get { return ltcPrice; }
            set { Set(ref ltcPrice, value); }
        }

        private string okbPersent;
        public string OKBPersent
        {
            get { return okbPersent; }
            set { Set(ref okbPersent, value); }
        }

        private string okbPrice;
        public string OKBPrice
        {
            get { return okbPrice; }
            set { Set(ref okbPrice, value); }
        }

        private string bnbPersent;
        public string BNBPersent
        {
            get { return bnbPersent; }
            set { Set(ref bnbPersent, value); }
        }

        private string bnbPrice;
        public string BNBPrice
        {
            get { return bnbPrice; }
            set { Set(ref bnbPrice, value); }
        }

        private string apiKeyEntity;
        public string ApiKeyEntity
        {
            get { return apiKeyEntity; }
            set { Set(ref apiKeyEntity, value); }
        }

        private string secretEntity;
        public string SecretEntity
        {
            get { return secretEntity; }
            set { Set(ref secretEntity, value); }
        }

        private string passPhraseEntity;
        public string PassPhraseEntity
        {
            get { return passPhraseEntity; }
            set { Set(ref passPhraseEntity, value); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { Set(ref title, value); }
        }

        //Max //\uEF2F \uEF2E
        private string maxstate;
        public string MaxState
        {
            get { return maxstate; }
            set { Set(ref maxstate, value); }
        }

        private double height;
        public double Height
        {
            get { return height; }
            set { Set(ref height, value); }
        }

        private double width;
        public double Width
        {
            get { return width; }
            set { Set(ref width, value); }
        }

        //max模式
        private Visibility maxvisibility;
        public Visibility MaxVisibility
        {
            get { return maxvisibility; }
            set { Set(ref maxvisibility, value); }
        }

        //mini模式
        private Visibility minivisibility;
        public Visibility MiniVisibility
        {
            get { return minivisibility; }
            set { Set(ref minivisibility, value); }
        }

        //E840 E77A
        private string pinstate;
        public string PinState
        {
            get { return pinstate; }
            set { Set(ref pinstate, value); }
        }

        private bool topmost;
        public bool TopMost
        {
            get { return topmost; }
            set { Set(ref topmost, value); }
        }

        //secret
        private Visibility secretvisibility;
        public Visibility SecretVisibility
        {
            get { return secretvisibility; }
            set { Set(ref secretvisibility, value); }
        }

        //Collective
        private string collective;
        public string Collective
        {
            get { return collective; }
            set { Set(ref collective, value); }
        }

        private BindableCollection<MarketTickerEntityModel> riseMarketTickerEntityModels = new BindableCollection<MarketTickerEntityModel>();
        public BindableCollection<MarketTickerEntityModel> RiseMarketTickerEntityModels
        {
            get { return riseMarketTickerEntityModels; }
            set { Set(ref riseMarketTickerEntityModels, value); }
        }

        private BindableCollection<MarketTickerEntityModel> fallMarketTickerEntityModels = new BindableCollection<MarketTickerEntityModel>();
        public BindableCollection<MarketTickerEntityModel> FallMarketTickerEntityModels
        {
            get { return fallMarketTickerEntityModels; }
            set { Set(ref fallMarketTickerEntityModels, value); }
        }

        #endregion

        #region Command

        #region Minimizeds

        public void Minimizeds()
        {
            WindowStates = WindowState.Minimized;
        }

        #endregion

        #region Maximizeds

        public void Maximizeds()
        {
            SmallMaximized();
        }

        #endregion

        #region Closeds

        public void Closeds()
        {
            Environment.Exit(0);
        }

        #endregion

        #region Ring

        public void Ring()
        {
            IoC.Get<MarketTickerViewModel>().StartMonitor();
        }

        #endregion

        #region Pin

        public void Pin()
        {
            SmallPin();
        }

        #endregion

        #region ShellViewLoad

        public void ShellViewLoad(FrameworkElement frameworkElement)
        {
            window = (Window)frameworkElement;
            window.MouseLeftButtonDown += delegate { window.DragMove(); };
            ////\uEF2F \uEF2E
            MaxState = "\uEF2E";
        }

        #endregion

        #region SmallMinized

        public void SmallMinized()
        {
            WindowStates = WindowState.Minimized;
        }

        #endregion

        #region SmallMaximized

        public void SmallMaximized()
        {
            if (WindowStates == WindowState.Maximized)
            {
                WindowStates = WindowState.Normal;
                MiniVisibility = Visibility.Visible;
                MaxVisibility = Visibility.Collapsed;
                IoC.Get<MarketTickerViewModel>().MaxContentVisibility = Visibility.Collapsed;
                IoC.Get<MarketTickerViewModel>().MiniContentVisibility = Visibility.Visible;
                MaxState = "\uEF2E";
            }
            else
            {
                WindowStates = WindowState.Maximized;
                MiniVisibility = Visibility.Collapsed;
                MaxVisibility = Visibility.Visible;
                IoC.Get<MarketTickerViewModel>().MaxContentVisibility = Visibility.Visible;
                IoC.Get<MarketTickerViewModel>().MiniContentVisibility = Visibility.Collapsed;
                MaxState = "\uEF2F";
            }
        }

        #endregion

        #region SmallClosed

        public void SmallClosed()
        {
            Environment.Exit(0);
        }

        #endregion

        #region SmallRing

        public void SmallRing()
        {
            IoC.Get<MarketTickerViewModel>().StartMonitor();
        }

        #endregion

        #region SmallPin

        public void SmallPin()
        {
            if (TopMost == false)
            {
                TopMost = true;
                PinState = "\uE840";
            }
            else
            {
                TopMost = false;
                PinState = "\uE77A";
            }
        }

        #endregion

        #region Secret

        public void Secret()
        {
            if (SecretVisibility == Visibility.Visible)
            {
                SecretVisibility = Visibility.Collapsed;
                if (!string.IsNullOrEmpty(ApiKeyEntity) && !string.IsNullOrEmpty(SecretEntity) && !string.IsNullOrEmpty(PassPhraseEntity))
                {
                    iniconfighelper.ConfigWriteValue("Config", "ApiKey", ApiKeyEntity);
                    iniconfighelper.ConfigWriteValue("Config", "Secret", SecretEntity);
                    iniconfighelper.ConfigWriteValue("Config", "PassPhrase", PassPhraseEntity);
                }
            }
            else
            {
                SecretVisibility = Visibility.Visible;
            }
        }

        #endregion

        #region

        public void SmallViewDebug()
        {
            bool logstate = IoC.Get<MarketTickerViewModel>().LogState;
            if (logstate == true)
            {
                IoC.Get<MarketTickerViewModel>().LogState = false;
            }
            else
            {
                IoC.Get<MarketTickerViewModel>().LogState = true;
            }
        }

        #endregion

        #endregion

        public void AccountBalance()
        {
            InitializeBackgroundWorker();
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
            }
        }

        public void PlayMedia()
        {
            string file = System.AppDomain.CurrentDomain.BaseDirectory + "1.mp3";
            mciSendString(string.Format("open \"{0}\" type mpegvideo alias media", file), null, 0, IntPtr.Zero);
            mciSendString("play media repeat", null, 0, IntPtr.Zero);
        }

        public void CloseMedia()
        {
            mciSendString("close media", null, 0, IntPtr.Zero);
            //mciSendString(@"close temp_alias", null, 0, 0);
        }

        public void SendEmail(string smtpServer, string Subject, string EmailContent)
        {
            //Send teh High priority Email  
            EmailManager mailMan = new EmailManager(smtpServer);
            EmailSendConfigure myConfig = new EmailSendConfigure();
            // replace with your email userName  
            myConfig.ClientCredentialUserName = "xx@outlook.com";
            // replace with your email account password
            myConfig.ClientCredentialPassword = "xx";
            myConfig.TOs = new string[] { "xx@live.com" };
            myConfig.CCs = new string[] { };
            myConfig.From = "xx@outlook.com";
            myConfig.FromDisplayName = "xx";
            myConfig.Priority = System.Net.Mail.MailPriority.Normal;
            myConfig.Subject = Subject;

            EmailContent myContent = new EmailContent();
            myContent.Content = EmailContent;

            mailMan.SendMail(myConfig, myContent);
        }

        /// <summary>
        /// 监听Bitcoin
        /// </summary>
        public async void MonitorBitcoin()
        {
            GetMonitorData("BTC");
        }

        /// <summary>
        /// 监听Ethereum
        /// </summary>
        public async void MonitorEthereum()
        {
            GetMonitorData("ETH");
        }

        /// <summary>
        /// 监听LTCcoin
        /// </summary>
        public async void MonitorLTCcoin()
        {
            GetMonitorData("LTC");
        }

        /// <summary>
        /// 监听OKBcoin
        /// </summary>
        public async void MonitorOKBcoin()
        {
            GetMonitorData("OKB");
        }

        /// <summary>
        /// 监听BNBcoin
        /// </summary>
        public async void MonitorBNBcoin()
        {
            GetMonitorData("BNB");
        }

        public async void GetMonitorData(string coinname)
        {
            try
            {
                Dictionary<string, string> requestParams = new Dictionary<string, string>();
                requestParams["instId"] = coinname + "-USDT";
                Task<string> task = marketDataApi.GetTicker(requestParams);
                string result = await task;
                if (result == null && string.IsNullOrEmpty(result)) { return; }
                MarketTickerModel marketTickerModel = JsonConvert.DeserializeObject<MarketTickerModel>(result);
                if (marketTickerModel.data != null)
                {
                    for (int i = 0; i < marketTickerModel.data.Length; i++)
                    {
                        //MarketTickerEntityModel marketTickerEntityModel = new MarketTickerEntityModel();
                        double price = Convert.ToDouble(marketTickerModel.data[i].last);
                        double openprice = Convert.ToDouble(marketTickerModel.data[i].open24h);
                        //跌涨幅
                        //最新成交价-24小时开盘价/24小时开盘价
                        var subtrahend = price - openprice;
                        var divisor = subtrahend / openprice;
                        var percentage = divisor * 100;
                        var percentageresult = percentage + "%";
                        var percentabs = Math.Abs(percentage);
                        switch (coinname)
                        {
                            case "BTC":
                                Persent = percentage.ToString() + "%";
                                BTCPrice = "$" + price;
                                if (percentabs >= 5)
                                {
                                    if (percentage > 0)
                                    {
                                        if (sendmailcount <= 3)
                                        {
                                            //上涨
                                            //播放音乐
                                            PlayMedia();
                                            //发送邮件
                                            string emailsubject = "BTC涨幅超过5%";
                                            string emailcontent = "比特币涨幅超过5%，可以减仓";
                                            SendEmail("smtp-mail.outlook.com", emailsubject, emailcontent);
                                            sendmailcount++;
                                        }
                                    }
                                    else
                                    {
                                        if (sendmailcount <= 3)
                                        {
                                            //下降
                                            //播放音乐
                                            PlayMedia();
                                            //发送邮件
                                            string emailsubject = "BTC跌幅超过5%";
                                            string emailcontent = "比特币跌幅超过5%，可以加仓";
                                            SendEmail("smtp-mail.outlook.com", emailsubject, emailcontent);
                                            sendmailcount++;
                                        }
                                    }
                                }
                                else
                                {
                                    CloseMedia();
                                    sendmailcount = -1;
                                }
                                break;
                            case "ETH":
                                ETHPersent = percentage.ToString() + "%";
                                ETHPrice = "$" + price;
                                break;
                            case "LTC":
                                LTCPersent = percentage.ToString() + "%";
                                LTCPrice = "$" + price;
                                break;
                            case "OKB":
                                OKBPersent = percentage.ToString() + "%";
                                OKBPrice = "$" + price;
                                break;
                            case "BNB":
                                BNBPersent = percentage.ToString() + "%";
                                BNBPrice = "$" + price;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public async void GetAccountBalance()
        {
            try
            {
                AccountApi accountApi = new AccountApi(ApiKeyEntity, SecretEntity, PassPhraseEntity);
                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls;
                Dictionary<string, string> requestParams = new Dictionary<string, string>();
                Task<string> task = accountApi.GetBalance(requestParams);
                string result = await task;
                AccountBalanceModel accountBalanceModel = JsonConvert.DeserializeObject<AccountBalanceModel>(result);
                //Debug.WriteLine("" + accountBalanceModel.data.Length);
                if (accountBalanceModel.data != null)
                {
                    for (int i = 0; i < accountBalanceModel.data[0].details.Length; i++)
                    {
                        TotalEq = "$" + accountBalanceModel.data[0].totalEq;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("OKXUI->GetAccountBalance->" + ex.Message, IoC.Get<MarketTickerViewModel>().LogState);
            }
        }

        #region  获取USDT

        private BindableCollection<AccountBalanceEntityModel> accountBalanceEntityModels = new BindableCollection<AccountBalanceEntityModel>();
        public BindableCollection<AccountBalanceEntityModel> AccountBalanceEntityModels
        {
            get { return accountBalanceEntityModels; }
            set { Set(ref accountBalanceEntityModels, value); }
        }

        public async void GetAccountBalanceData()
        {
            AccountApi accountApi = new AccountApi(ApiKeyEntity, SecretEntity, PassPhraseEntity);
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

        #endregion

        public async void TradeOrder()
        {
            TradeApi tradeApi = new TradeApi(ApiKeyEntity, SecretEntity, PassPhraseEntity);
            Dictionary<string, string> requestParameters = new Dictionary<string, string>();
            requestParameters["instId"] = "ADA-USDT";
            requestParameters["tdMode"] = "cash";
            requestParameters["ccy"] = "";
            requestParameters["clOrdId"] = "";
            requestParameters["tag"] = "";
            requestParameters["side"] = "sell";
            requestParameters["posSide"] = "";
            requestParameters["ordType"] = "limit";
            requestParameters["sz"] = "84";
            requestParameters["px"] = "1.5";
            requestParameters["reduceOnly"] = "";
            Task<string> task = tradeApi.PlaceOrder(requestParameters);
            var result = await task;
            //PlaceOrderEntityModel
            PlaceOrderEntityModel assetCurrenciesModel = JsonConvert.DeserializeObject<PlaceOrderEntityModel>(result);
            if (assetCurrenciesModel.data != null)
            {
                if (assetCurrenciesModel.data[0].sCode == "0")
                {
                    //成功
                }
            }
        }

        public void AssetCurrencies()
        {
            navigationService.NavigateToViewModel(typeof(AssetCurrenciesViewModel));
        }

        public void AssetBalances()
        {
            navigationService.NavigateToViewModel(typeof(AssetBalancesViewModel));
        }

        public void AssetAssetValuation()
        {
            navigationService.NavigateToViewModel(typeof(AssetAssetValuationViewModel));
        }

        public async void AccountPositions()
        {
            //GetPositions
            AccountApi accountApi = new AccountApi(ApiKeyEntity, SecretEntity, PassPhraseEntity);
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls;
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            Task<string> task = accountApi.GetPositions(requestParams);
            string result = await task;
        }

        public async void AccountConfig()
        {
            AccountApi accountApi = new AccountApi(ApiKeyEntity, SecretEntity, PassPhraseEntity);
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls;
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            Task<string> task = accountApi.GetAccountConfiguration(requestParams);
            string result = await task;
            AccountConfigModel accountConfigModel = JsonConvert.DeserializeObject<AccountConfigModel>(result);
        }

        public async void MarketTickers()
        {
            MarketDataApi marketDataApi = new MarketDataApi(ApiKeyEntity, SecretEntity, PassPhraseEntity);
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            requestParams["instType"] = "SPOT";
            Task<string> task = marketDataApi.GetTickers(requestParams);
            string result = await task;
            MarketTickersModel marketTickersModel = JsonConvert.DeserializeObject<MarketTickersModel>(result);
        }

        public void MarketTicker()
        {
            navigationService.NavigateToViewModel(typeof(MarketTickerViewModel));
        }

        public async void MarketIndexTickers()
        {
            //quoteCcy	String	可选	指数计价单位， 目前只有 USD/USDT/BTC为计价单位的指数，quoteCcy和instId必须填写一个
            //instId String  可选 指数，如 BTC-USD
            MarketDataApi marketDataApi = new MarketDataApi(ApiKeyEntity, SecretEntity, PassPhraseEntity);
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            //requestParams["quoteCcy"] = "LINK";
            requestParams["instId"] = "LINK-USD";
            Task<string> task = marketDataApi.GetIndexTickers(requestParams);
            string result = await task;
            MarketIndexTickersModel marketIndexTickersModel = JsonConvert.DeserializeObject<MarketIndexTickersModel>(result);
        }
    }
}
