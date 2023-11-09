using Caliburn.Micro;
using Newtonsoft.Json;
using OKX.UI.Helper;
using OKX.UI.Models;
using OKX.UI.Models.Entitys.AccountBalance;
using OKX.UI.Models.Entitys.HistoryCandles;
using OKX.UI.Models.Entitys.MarketTicker;
using OKX.UI.Models.Entitys.MarketTickers;
using OKX.UI.Models.Entitys.MaximumAvailableTradableAmount;
using OKX.UI.Models.Entitys.MaximumAvailableTradableAmounts;
using OKX.UI.Models.Entitys.PlaceOrder;
using OKX.UI.Models.Entitys.RiseAndFall;
using OKXSDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace OKX.UI.ViewModels
{
    public class MarketTickerViewModel : Screen
    {
        public MarketTickerViewModel()
        {
            factorslist = new List<FactorModel>();
            MaxContentVisibility = Visibility.Visible;
            MiniContentVisibility = Visibility.Visible;
            iniconfighelper = new IniConfigHelper(iniconfigpath);
            if (!File.Exists(iniconfigpath))
            {
                iniconfighelper.ConfigWriteValue("Config", "BuyPrice", "0");
                iniconfighelper.ConfigWriteValue("Config", "ApiKey", "NULL");
                iniconfighelper.ConfigWriteValue("Config", "Secret", "NULL");
                iniconfighelper.ConfigWriteValue("Config", "PassPhrase", "NULL");
                //iniconfighelper.ConfigWriteValue("Gate", "Collective", "BTC,ETH,BNB,LTC,LINK,ATOM,ETC,POND,LINA,EOS,XLM,AVAX,BCH,UNI,SUSHI,AAVE,XMR,DASH,ZEC,IOTX,IOST,1INCH,ADA,SCRT,ZEN,OSMO");
                iniconfighelper.ConfigWriteValue("Config", "Collective", "BTC,ETH,BNB,LTC,LINK,ATOM,ETC,EOS,XLM,AVAX,BCH,UNI,SUSHI,AAVE,XMR,DASH,ZEC,IOST,1INCH,ADA,ZEN");
                collective = iniconfighelper.ConfigReadValue("Config", "Collective");
                collectives = collective.Split(',');
            }
            else
            {
                apiKey = iniconfighelper.ConfigReadValue("Config", "ApiKey");
                secret = iniconfighelper.ConfigReadValue("Config", "Secret");
                passPhrase = iniconfighelper.ConfigReadValue("Config", "PassPhrase");
                collective = iniconfighelper.ConfigReadValue("Config", "Collective");
                collectives = collective.Split(',');
                marketDataApi = new MarketDataApi(apiKey, secret, passPhrase);
                accountApi = new AccountApi(apiKey, secret, passPhrase);
                tradeApi = new TradeApi(apiKey, secret, passPhrase);
            }

            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls;
            InitializeBackgroundWorker();
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
            }

            //TEST
            var windowsstate = IoC.Get<ShellViewModel>().WindowStates;
            if (windowsstate == WindowState.Maximized)
            {
                MaxContentVisibility = Visibility.Visible;
                MiniContentVisibility = Visibility.Collapsed;
            }
            else
            {
                MaxContentVisibility = Visibility.Collapsed;
                MiniContentVisibility = Visibility.Visible;
            }
            LogState = false;
            ////设置的间隔为一分钟
            //dispatchertimer.Interval = new TimeSpan(0, 0, 5);
            ////设置定时器溢出触发事件
            //dispatchertimer.Tick += DispatchertimerTick;
            // //开启定时器
            //dispatchertimer.IsEnabled = true;
            //dispatchertimer.Start();
        }

        private void DispatchertimerTick(object sender, EventArgs e)
        {
            MonitorMarket();
        }

        #region 变量

        public static int fallcount = 0;

        public static int risecount = 0;

        public double buyAvailableTradableAmount;

        public double sellAvailableTradableAmount;

        MarketDataApi marketDataApi;

        AccountApi accountApi;

        TradeApi tradeApi;

        IniConfigHelper iniconfighelper;

        string iniconfigpath = System.AppDomain.CurrentDomain.BaseDirectory + "Config\\Config.ini";

        string coincollectionpath = System.AppDomain.CurrentDomain.BaseDirectory + "Config\\Config.txt";

        private string[] collectives = null;

        private string collective = "";

        public double trackrecordprice;

        //private string apiKey = "f8f6a157-357a-40f6-ad3a-ef72f2fa9003";
        //private string secret = "DCB9C33D6DD32E0F04C9B891435DDDA1";
        //private string passPhrase = "test321";

        private string apiKey = "";

        private string secret = "";

        private string passPhrase = "";

        //double availBal = -1;
        double availEq = -1;

        string ccy;

        int sendmailcount = -1;

        List<FactorModel> factorslist;

        DispatcherTimer dispatchertimer = new DispatcherTimer();

        #endregion

        #region BackgroundWorker

        private ManualResetEvent manualReset = new ManualResetEvent(true);
        //开始执行后台操作
        //backgroundWorker.RunWorkerAsync();
        private BackgroundWorker backgroundWorker;
        //manualReset.Reset();//暂停当前线程的工作，发信号给waitOne方法，阻塞
        //manualReset.Set();//继续当前线程的工作，发信号给waitOne方法，继续运行Waitone后面的代码
        //backgroundWorker.CancelAsync();//结束后台线程

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
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorkerDoWork);
            //执行ReportProgress方法后触发ProgressChanged，将执行ProgressChanged方法中的代码
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorkerProgressChanged);
            //异步操作完成或取消时执行的操作，当调用DoWork事件执行完成时触发。 
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorkerRunWorkerCompleted);
            #endregion
        }

        //backgroundWorker_DoWork完成后执行
        private void backgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result == true)
            {

            }
        }

        private void backgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        //backgroundWorker_DoWork是运行在非UI线程上的，因此该内部代码应该避免与UI界面交互，交互的逻辑应该放到
        //ProgressChanged和RunWorkerCompleted事件中
        private void backgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            //e.Result = true;
            while (true)
            {
                //执行查询()
                ExecuteRise();
                ExecuteFall();
                //监听BTC
                //MonitorBitcoin();
                //MonitorMarket();
                Thread.Sleep(110);
            }
        }

        #endregion

        #region Command

        public void Fall()
        {
            fallcount++;
            if (fallcount % 2 == 0)
            {
                FallVisibilitys = Visibility.Collapsed;
            }
            else
            {
                FallVisibilitys = Visibility.Visible;
            }

        }

        public void Rise()
        {
            risecount++;
            if (risecount % 2 == 0)
            {

                RiseVisibilitys = Visibility.Collapsed;
            }
            else
            {
                RiseVisibilitys = Visibility.Visible;
            }
        }

        public void RiseAddTicker()
        {
            RiseFallEntityModel riseFallEntityModel = new RiseFallEntityModel();
            riseFallEntityModel.CoinName = "";
            riseFallEntityModel.CoinPrice = 0;
            RiseEntityModels.Add(riseFallEntityModel);
        }

        public void RiseDeleteTicker()
        {
            var deletelist = RiseEntityModels.ToList();
            var deleteindex = deletelist.FindIndex(x => x.CoinName == RiseEntityModel.CoinName);
            RiseEntityModels.RemoveAt(deleteindex);
        }

        public void FallAddTicker()
        {
            RiseFallEntityModel riseFallEntityModel = new RiseFallEntityModel();
            riseFallEntityModel.CoinName = "";
            riseFallEntityModel.CoinPrice = 0;
            FallEntityModels.Add(riseFallEntityModel);
        }

        public void FallDeleteTicker()
        {
            var deletelist = FallEntityModels.ToList();
            var deleteindex = deletelist.FindIndex(x => x.CoinName == FallEntityModel.CoinName);
            FallEntityModels.RemoveAt(deleteindex);
        }

        #endregion

        #region Action

        public void StartMonitor()
        {
            if (File.Exists(coincollectionpath))
            {
                string[] coincollection = File.ReadAllLines(coincollectionpath);
                if (coincollection != null && coincollection.Length > 0)
                {
                    for (int i = 0; i < coincollection.Length; i++)
                    {
                        string[] coinsplit = coincollection[i].Split(',');
                        RiseFallEntityModel fallmodel = new RiseFallEntityModel();
                        fallmodel.CoinName = coinsplit[0];
                        fallmodel.CoinPrice = Convert.ToDouble(coinsplit[1]);
                        FallEntityModels.Add(fallmodel);
                        RiseFallEntityModel risemodel = new RiseFallEntityModel();
                        risemodel.CoinName = coinsplit[0];
                        risemodel.CoinPrice = Convert.ToDouble(coinsplit[2]);
                        RiseEntityModels.Add(risemodel);
                    }
                }
            }

        }

        public void MonitorMarket()
        {
            //new TaskFactory().StartNew(() =>
            //{
            //while (true)
            //{
            //Thread.Sleep(101);
            if (collectives != null && collectives.Length > 0)
            {
                for (int i = 0; i < collectives.Length; i++)
                {
                    MonitorFactor(collectives[i]);
                }
            }
            //}
            //});
        }

        private void AddFactorData(string coinname, string percentageresult)
        {
            FactorModel factor = new FactorModel();
            factor.Name = coinname;
            factor.Percentage = percentageresult;
            var isexis = factorslist.Where(x => x.Name == coinname).ToList();
            var index = factorslist.FindIndex(x => x.Name == coinname);
            if (isexis.Count == 0)
            {
                factorslist.Add(factor);
            }
            else
            {
                factorslist[index].Percentage = percentageresult;
            }
        }

        public async void MonitorFactor(string coinname)
        {
            //OnUIThread(async () => { });
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            try
            {
                //requestParams["instId"] = "BTC-USDT";
                requestParams["instId"] = coinname + "-USDT";
                Task<string> task = marketDataApi.GetTicker(requestParams);
                string result = await task;
                if (result == null&&string.IsNullOrEmpty(result)) { return; }
                MarketTickerModel marketTickerModel = JsonConvert.DeserializeObject<MarketTickerModel>(result);
                if (marketTickerModel.data != null)
                {
                    for (int i = 0; i < marketTickerModel.data.Length; i++)
                    {
                        double price = Convert.ToDouble(marketTickerModel.data[i].last);
                        double openprice = Convert.ToDouble(marketTickerModel.data[i].open24h);
                        //最新成交价-24小时开盘价/24小时开盘价
                        var subtrahend = price - openprice;
                        var divisor = subtrahend / openprice;
                        var percentage = divisor * 100;
                        var percentageresult = percentage + "%";
                        var percentabs = Math.Abs(percentage);
                        //BTC
                        if (percentabs >= 5 && coinname == "BTC")
                        {
                            if (percentage > 0)
                            {
                                //上涨
                                AddFactorData(coinname, percentageresult);
                            }
                            else
                            {
                                //下跌
                                AddFactorData(coinname, percentageresult);
                            }
                        }
                        //山寨
                        if (percentabs >= 15 && coinname != "BTC")
                        {
                            if (percentage > 0)
                            {
                                //上涨
                                AddFactorData(coinname, percentageresult);
                            }
                            else
                            {
                                //下跌
                                AddFactorData(coinname, percentageresult);
                            }
                        }
                        //清空
                        if (percentabs < 5 && coinname == "BTC")
                        {
                            CloseMedia();
                            sendmailcount = -1;
                            factorslist.Clear();
                        }
                        //响应
                        if (factorslist.Count >= 2)
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
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("MonitorFactor ex:" + ex.Message, LogState);
                LogHelper.WriteLog("MonitorFactor requestParams[\"instId\"]:" + requestParams["instId"], LogState);
            }

        }

        /// <summary>
        /// 监听Bitcoin
        /// </summary>
        public async void MonitorBitcoin()
        {
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            requestParams["instId"] = "BTC-USD-SWAP";
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
                    //marketTickerEntityModel.Amplitude = percentageresult;
                    //RiseAmplitude = percentageresult;
                }
            }
        }

        public void ExecuteRise()
        {
            //10/1s
            //GetTicker
            OnUIThread(async () =>
            {
                LogHelper.WriteLog("ExecuteRise 0", LogState);
                if (RiseMarketTickerEntityModels.Count > 10)
                {
                    LogHelper.WriteLog("ExecuteRise 1", LogState);
                    RiseMarketTickerEntityModels.Clear();
                    LogHelper.WriteLog("ExecuteRise 2", LogState);
                    IoC.Get<ShellViewModel>().RiseMarketTickerEntityModels.Clear();
                    LogHelper.WriteLog("ExecuteRise 3", LogState);
                }

                if (RiseEntityModels.Count > 0)
                {
                    LogHelper.WriteLog("ExecuteRise 4", LogState);
                    for (int m = 0; m < RiseEntityModels.Count; m++)
                    {
                        LogHelper.WriteLog("ExecuteRise 5", LogState);
                        try
                        {
                            LogHelper.WriteLog("ExecuteRise 6", LogState);
                            if (string.IsNullOrEmpty(RiseEntityModels[m].CoinName))
                            {
                                return;
                            }
                            LogHelper.WriteLog("ExecuteRise 7", LogState);
                            Dictionary<string, string> requestParams = new Dictionary<string, string>();
                            LogHelper.WriteLog("ExecuteRise 8", LogState);
                            //requestParams["instId"] = RiseEntityModels[m].CoinName + "-USD-SWAP";
                            requestParams["instId"] = RiseEntityModels[m].CoinName + "-USDT";
                            LogHelper.WriteLog("ExecuteRise 9", LogState);
                            Task<string> task = marketDataApi.GetTicker(requestParams);
                            LogHelper.WriteLog("ExecuteRise 10", LogState);
                            string result = await task;
                            if (result == null && string.IsNullOrEmpty(result)) { return; }
                            LogHelper.WriteLog("ExecuteRise 11", LogState);
                            //var result = task.Result;
                            //监听BTC
                            MarketTickerModel marketTickerModel = JsonConvert.DeserializeObject<MarketTickerModel>(result);
                            LogHelper.WriteLog("ExecuteRise 12", LogState);
                            if (marketTickerModel.data != null)
                            {
                                LogHelper.WriteLog("ExecuteRise 13", LogState);
                                for (int i = 0; i < marketTickerModel.data.Length; i++)
                                {
                                    LogHelper.WriteLog("ExecuteRise 14", LogState);
                                    MarketTickerEntityModel marketTickerEntityModel = new MarketTickerEntityModel();
                                    marketTickerEntityModel.instType = marketTickerModel.data[i].instType;

                                    marketTickerEntityModel.instId = marketTickerModel.data[i].instId;

                                    if (!string.IsNullOrEmpty(marketTickerEntityModel.instId))
                                    {
                                        string[] instIdArray = marketTickerEntityModel.instId.Split('-');
                                        marketTickerEntityModel.instId = instIdArray[0];
                                    }

                                    marketTickerEntityModel.last = marketTickerModel.data[i].last;
                                    double price = Convert.ToDouble(marketTickerModel.data[i].last);
                                    double openprice = Convert.ToDouble(marketTickerModel.data[i].open24h);
                                    //跌涨幅
                                    //最新成交价-24小时开盘价/24小时开盘价
                                    var subtrahend = price - openprice;
                                    var divisor = subtrahend / openprice;
                                    var percentage = divisor * 100;
                                    var percentageresult = percentage + "%";

                                    //marketTickerEntityModel.Amplitude = percentageresult;
                                    RiseAmplitude = percentageresult;

                                    double avaliprice = RiseEntityModels[m].CoinPrice + (RiseEntityModels[m].CoinPrice * RisePercentage);
                                    //if (price >= RiseEntityModels[m].CoinPrice)
                                    //20230410
                                    //price 最新成交价



                                    if (price >= avaliprice)
                                    {
                                        LogHelper.WriteLog(" sell price:" + price, LogState);
                                        LogHelper.WriteLog(" sell avaliprice:" + avaliprice, LogState);
                                        LogHelper.WriteLog(" 跌涨幅 percentageresult:" + percentageresult, LogState);
                                        double buyprice = Convert.ToDouble(iniconfighelper.ConfigReadValue("Config", "BuyPrice"));
                                        //if (buyprice > 0)
                                        //{
                                        //    HistoryCandles(price, RiseEntityModels[m].CoinName);
                                        //}
                                        if (RiseEntityModels[m].CoinPrice > 0)
                                        {
                                            AvailableTradableAmount(RiseEntityModels[m].CoinName + "-USDT", price);
                                            TradeOrder(RiseEntityModels[m].CoinName + "-USDT", "sell", price.ToString());
                                        }
                                        ////播放音乐
                                        //PlayMedia();
                                        ////发送邮件
                                        //string emailsubject = "BTC涨幅超过5%";
                                        //string emailcontent = "比特币涨幅超过5%，可以减仓";
                                        //SendEmail("smtp-mail.outlook.com", emailsubject, emailcontent);
                                    }
                                    else
                                    {
                                        //CloseMedia();
                                    }

                                    marketTickerEntityModel.lastSz = marketTickerModel.data[i].lastSz;
                                    marketTickerEntityModel.askPx = marketTickerModel.data[i].askPx;
                                    marketTickerEntityModel.askSz = marketTickerModel.data[i].askSz;
                                    marketTickerEntityModel.bidPx = marketTickerModel.data[i].bidPx;
                                    marketTickerEntityModel.bidSz = marketTickerModel.data[i].bidSz;
                                    marketTickerEntityModel.open24h = marketTickerModel.data[i].open24h;
                                    marketTickerEntityModel.high24h = marketTickerModel.data[i].high24h;
                                    marketTickerEntityModel.low24h = marketTickerModel.data[i].low24h;
                                    marketTickerEntityModel.volCcy24h = marketTickerModel.data[i].volCcy24h;
                                    marketTickerEntityModel.vol24h = marketTickerModel.data[i].vol24h;
                                    marketTickerEntityModel.ts = marketTickerModel.data[i].ts;
                                    marketTickerEntityModel.sodUtc0 = marketTickerModel.data[i].sodUtc0;
                                    marketTickerEntityModel.sodUtc8 = marketTickerModel.data[i].sodUtc8;
                                    RiseMarketTickerEntityModels.Add(marketTickerEntityModel);
                                    IoC.Get<ShellViewModel>().Title = marketTickerEntityModel.instId + "|" + marketTickerEntityModel.last + "|" + percentageresult;
                                    IoC.Get<ShellViewModel>().RiseMarketTickerEntityModels.Add(marketTickerEntityModel);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog("ExecuteRise:" + ex.Message, LogState);
                        }
                    }
                }
            });
        }

        public void ExecuteFall()
        {
            //10/1s
            //GetTicker
            OnUIThread(async () =>
            {
                LogHelper.WriteLog("ExecuteFall 0", LogState);
                if (FallMarketTickerEntityModels.Count > 10)
                {
                    LogHelper.WriteLog("ExecuteFall 1", LogState);
                    FallMarketTickerEntityModels.Clear();
                    LogHelper.WriteLog("ExecuteFall 2", LogState);
                    IoC.Get<ShellViewModel>().FallMarketTickerEntityModels.Clear();
                    LogHelper.WriteLog("ExecuteFall 3", LogState);
                }
                LogHelper.WriteLog("ExecuteFall 0", LogState);
                if (FallEntityModels.Count > 0)
                {
                    LogHelper.WriteLog("ExecuteFall 4", LogState);
                    for (int m = 0; m < FallEntityModels.Count; m++)
                    {
                        LogHelper.WriteLog("ExecuteFall 5", LogState);
                        try
                        {
                            LogHelper.WriteLog("ExecuteFall 6", LogState);
                            if (string.IsNullOrEmpty(FallEntityModels[m].CoinName))
                            {
                                return;
                            }
                            LogHelper.WriteLog("ExecuteFall 7", LogState);
                            Dictionary<string, string> requestParams = new Dictionary<string, string>();
                            LogHelper.WriteLog("ExecuteFall 8", LogState);
                            requestParams["instId"] = FallEntityModels[m].CoinName + "-USDT";
                            LogHelper.WriteLog("ExecuteFall 9", LogState);
                            Task<string> task = marketDataApi.GetTicker(requestParams);
                            LogHelper.WriteLog("ExecuteFall 10", LogState);
                            string result = await task;
                            if (result == null && string.IsNullOrEmpty(result)) { return; }
                            LogHelper.WriteLog("ExecuteFall 11", LogState);
                            //var result = task.Result;
                            MarketTickerModel marketTickerModel = JsonConvert.DeserializeObject<MarketTickerModel>(result);
                            LogHelper.WriteLog("ExecuteFall 12", LogState);
                            if (marketTickerModel.data != null)
                            {
                                LogHelper.WriteLog("ExecuteFall 13", LogState);
                                for (int i = 0; i < marketTickerModel.data.Length; i++)
                                {
                                    LogHelper.WriteLog("ExecuteFall 14", LogState);
                                    MarketTickerEntityModel marketTickerEntityModel = new MarketTickerEntityModel();
                                    marketTickerEntityModel.instType = marketTickerModel.data[i].instType;
                                    marketTickerEntityModel.instId = marketTickerModel.data[i].instId;
                                    if (!string.IsNullOrEmpty(marketTickerEntityModel.instId))
                                    {
                                        string[] instIdArray = marketTickerEntityModel.instId.Split('-');
                                        marketTickerEntityModel.instId = instIdArray[0];
                                    }
                                    marketTickerEntityModel.last = marketTickerModel.data[i].last;
                                    double price = Convert.ToDouble(marketTickerModel.data[i].last);
                                    double openprice = Convert.ToDouble(marketTickerModel.data[i].open24h);

                                    //跌涨幅
                                    //最新成交价-24小时开盘价/24小时开盘价
                                    var subtrahend = price - openprice;
                                    var divisor = subtrahend / openprice;
                                    var percentage = divisor * 100;
                                    var percentageresult = percentage + "%";

                                    //marketTickerEntityModel.Amplitude = percentageresult;
                                    FallAmplitude = percentageresult;

                                    double avaliprice = FallEntityModels[m].CoinPrice - (FallEntityModels[m].CoinPrice * FallPercentage);
                                    //if (price <= FallEntityModels[m].CoinPrice)
                                    //20230410
                                    //price 最新成交价


                                    if (price <= avaliprice)
                                    {
                                        LogHelper.WriteLog(" buy price:" + price, LogState);
                                        LogHelper.WriteLog(" buy avaliprice:" + avaliprice, LogState);
                                        LogHelper.WriteLog(" 跌涨幅 percentageresult:" + percentageresult, LogState);
                                        //20230410
                                        //price 最新成交价
                                        double buyprice = Convert.ToDouble(iniconfighelper.ConfigReadValue("Config", "BuyPrice"));
                                        //if (buyprice > 0)
                                        //{
                                        //    HistoryCandles(price, RiseEntityModels[m].CoinName);
                                        //}
                                        if (FallEntityModels[m].CoinPrice > 0)
                                        {
                                            AvailableTradableAmount(FallEntityModels[m].CoinName + "-USDT", price);
                                            TradeOrder(FallEntityModels[m].CoinName + "-USDT", "buy", price.ToString());
                                        }
                                        ////播放音乐
                                        //PlayMedia();
                                        ////发送邮件
                                        //string emailsubject = "BTC 跌幅超过";
                                        //string emailcontent ="比特币跌幅超过5%，可以加仓";
                                        //SendEmail("smtp-mail.outlook.com", emailsubject, emailcontent);
                                    }
                                    else
                                    {
                                        //CloseMedia();
                                    }

                                    marketTickerEntityModel.lastSz = marketTickerModel.data[i].lastSz;
                                    marketTickerEntityModel.askPx = marketTickerModel.data[i].askPx;
                                    marketTickerEntityModel.askSz = marketTickerModel.data[i].askSz;
                                    marketTickerEntityModel.bidPx = marketTickerModel.data[i].bidPx;
                                    marketTickerEntityModel.bidSz = marketTickerModel.data[i].bidSz;
                                    marketTickerEntityModel.open24h = marketTickerModel.data[i].open24h;
                                    marketTickerEntityModel.high24h = marketTickerModel.data[i].high24h;
                                    marketTickerEntityModel.low24h = marketTickerModel.data[i].low24h;
                                    marketTickerEntityModel.volCcy24h = marketTickerModel.data[i].volCcy24h;
                                    marketTickerEntityModel.vol24h = marketTickerModel.data[i].vol24h;
                                    marketTickerEntityModel.ts = marketTickerModel.data[i].ts;
                                    marketTickerEntityModel.sodUtc0 = marketTickerModel.data[i].sodUtc0;
                                    marketTickerEntityModel.sodUtc8 = marketTickerModel.data[i].sodUtc8;
                                    FallMarketTickerEntityModels.Add(marketTickerEntityModel);
                                    IoC.Get<ShellViewModel>().Title = marketTickerEntityModel.instId + "|" + marketTickerEntityModel.last + "|" + percentageresult;
                                    IoC.Get<ShellViewModel>().FallMarketTickerEntityModels.Add(marketTickerEntityModel);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog("ExecuteFall:" + ex.Message, LogState);
                        }
                    }
                }
            });
        }

        //GetMaximumAvailableTradableAmount//获取最大可买可卖的数量
        public async void AvailableTradableAmount(string instId,double lastprice)
        {
            try
            {
                double results = 0;

                Dictionary<string, string> requestParameters = new Dictionary<string, string>();
                requestParameters["instId"] = instId; //"ADA-USDT";
                requestParameters["tdMode"] = "cash";
                //requestParameters["ccy"] = "";
                //requestParameters["reduceOnly"] = "";
                //requestParameters["px"] = "";
                //AccountApi accountApi = new AccountApi(apiKey, secret, passPhrase);
                //获取方案一
                Task<string> task = accountApi.GetMaximumTradableSizeForInstrument(requestParameters);
                var result = await task;
                MaximumAvailableTradableAmountsEntityModel maximumAvailableTradableAmount = JsonConvert.DeserializeObject<MaximumAvailableTradableAmountsEntityModel>(result);

                if (maximumAvailableTradableAmount.code == "0")
                {
                    try
                    {
                        var maxBuy = maximumAvailableTradableAmount.data[0].maxBuy;
                        var maxSell = maximumAvailableTradableAmount.data[0].maxSell;
                        double buy = Convert.ToDouble(maxBuy);
                        double sell = Convert.ToDouble(maxSell);
                        if (buy > 0.1)
                        {
                            results = buy;
                            buyAvailableTradableAmount = buy;
                            trackrecordprice = buy;
                        }
                        if (buy < 0.1)
                        {
                            if (ccy != "USDT" && availEq > 0.1)
                            {
                                sellAvailableTradableAmount = availEq;
                            }
                        }
                        if (buy<0.1&& sell>0)
                        {
                            var pieces = sellAvailableTradableAmount / lastprice;
                            sellAvailableTradableAmount = pieces;
                        }
                                
                        //if (sell > 0.1)
                        //{
                        //    results = sell;
                        //    sellAvailableTradableAmount = sell;
                        //    if (buy > 0.1)
                        //    {
                        //        trackrecordprice = buy;
                        //    }
                        //}
                        LogHelper.WriteLog(" AvailableTradableAmount  maxBuy:" + maxBuy, LogState);
                        LogHelper.WriteLog(" AvailableTradableAmount  maxSell:" + maxSell, LogState);
                        LogHelper.WriteLog(" AvailableTradableAmount  buyAvailableTradableAmount:" + buyAvailableTradableAmount, LogState);
                        LogHelper.WriteLog(" AvailableTradableAmount  sellAvailableTradableAmount:" + sellAvailableTradableAmount, LogState);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(":" + ex.Message, LogState);
                    }
                }

                #region //获取方案二

                //Task<string> task = accountApi.GetMaximumAvailableTradableAmount(requestParameters);
                //var result = await task;
                //MaximumAvailableTradableAmountEntityModel maximumAvailableTradableAmount = JsonConvert.DeserializeObject<MaximumAvailableTradableAmountEntityModel>(result);
                //if (maximumAvailableTradableAmount.code == "0")
                //{
                //    try
                //    {
                //        var availBuy = maximumAvailableTradableAmount.data[0].availBuy;
                //        var availSell = maximumAvailableTradableAmount.data[0].availSell;
                //        double buy = Convert.ToDouble(availBuy);
                //        double sell = Convert.ToDouble(availSell);
                //        if (buy > 1)
                //        {
                //            results = buy;
                //            buyAvailableTradableAmount = buy;
                //        }
                //        if (sell > 1)
                //        {
                //            results = sell;
                //            sellAvailableTradableAmount = sell;
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        LogHelper.WriteLog(":" + ex.Message,LogState);
                //    }
                //}
                //availableTradableAmount = results;

                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("->AvailableTradableAmount" + ex.Message, LogState);
            }
        }

        public async void TradeOrder(string instId, string side, string pricelast)
        {
            //await  Application.Current.Dispatcher.BeginInvoke(() =>
            // {
            try
            {
                if (side == "buy" && buyAvailableTradableAmount <= 0)
                {
                    LogHelper.WriteLog(" TradeOrder side == \"buy\"&& buyAvailableTradableAmount<=0:", LogState);
                    LogHelper.WriteLog(" TradeOrder side:" + side, LogState);
                    LogHelper.WriteLog(" TradeOrder buyAvailableTradableAmount:" + buyAvailableTradableAmount, LogState);
                    return;
                }
                if (side == "sell" && sellAvailableTradableAmount <= 0)
                {
                    LogHelper.WriteLog(" TradeOrder side == \"sell\"&& sellAvailableTradableAmount<=0:", LogState);
                    LogHelper.WriteLog(" TradeOrder side:" + side, LogState);
                    LogHelper.WriteLog(" TradeOrder buyAvailableTradableAmount:" + sellAvailableTradableAmount, LogState);
                    return;
                }

                Dictionary<string, string> requestParameters = new Dictionary<string, string>();
                requestParameters["instId"] = instId; //"ADA-USDT";
                requestParameters["tdMode"] = "cash";
                requestParameters["ccy"] = "";
                requestParameters["clOrdId"] = "";
                requestParameters["tag"] = "";
                requestParameters["side"] = side;
                requestParameters["posSide"] = "";
                requestParameters["ordType"] = "limit";

                if (side == "buy")
                {
                    requestParameters["sz"] = buyAvailableTradableAmount.ToString();
                }
                if (side == "sell")
                {
                    if (sellAvailableTradableAmount > 0.1)
                    {
                        requestParameters["sz"] = sellAvailableTradableAmount.ToString();
                    }
                }

                requestParameters["px"] = pricelast; //"1.5";
                requestParameters["reduceOnly"] = "";
                requestParameters["tgtCcy"] = "";
                requestParameters["banAmend"] = "";
                double re = -1;
                if (!string.IsNullOrEmpty(requestParameters["sz"]))
                {
                    re = Convert.ToDouble(requestParameters["sz"]);
                }
                if (re < 0)
                {
                    LogHelper.WriteLog(" TradeOrder re<0:", LogState);
                    LogHelper.WriteLog(" TradeOrder  re<0 trackrecordprice:" + trackrecordprice, LogState);
                    return;
                }

                Task<string> task = tradeApi.PlaceOrder(requestParameters);

                var result = await task;

                LogHelper.WriteLog(" TradeOrder trackrecordprice:" + trackrecordprice, LogState);
                LogHelper.WriteLog(" TradeOrder instId:" + instId, LogState);
                LogHelper.WriteLog(" TradeOrder side:" + side, LogState);
                LogHelper.WriteLog(" TradeOrder buy sz:" + buyAvailableTradableAmount.ToString(), LogState);
                LogHelper.WriteLog(" TradeOrder sell sz:" + sellAvailableTradableAmount.ToString(), LogState);
                LogHelper.WriteLog(" TradeOrder px:" + pricelast, LogState);
                PlaceOrderEntityModel assetCurrenciesModel = JsonConvert.DeserializeObject<PlaceOrderEntityModel>(result);
                if (assetCurrenciesModel.data != null)
                {
                    if (assetCurrenciesModel.data[0].sCode == "0")
                    {
                        //成功
                        LogHelper.WriteLog(" 成功", LogState);
                        if (side == "buy")
                        {
                            iniconfighelper.ConfigWriteValue("Config", "BuyPrice", pricelast);
                            buyAvailableTradableAmount = 0;    
                            //trackrecordprice = buyAvailableTradableAmount;
                        }
                        if (side == "sell")
                        {
                            sellAvailableTradableAmount = 0;
                            //trackrecordprice = buyAvailableTradableAmount;
                        }
                        LogHelper.WriteLog(" trackrecordprice:" + trackrecordprice, LogState);
                        //if (side == "sell")
                        //{
                        //    requestParameters["sz"] = sellAvailableTradableAmount.ToString();
                        //}
                    }
                    else
                    {
                        LogHelper.WriteLog("失败", LogState);
                        LogHelper.WriteLog("失败 clOrdId:" + assetCurrenciesModel.data[0].clOrdId, LogState);
                        LogHelper.WriteLog("失败 ordId:" + assetCurrenciesModel.data[0].ordId, LogState);
                        LogHelper.WriteLog("失败 sMsg:" + assetCurrenciesModel.data[0].sMsg, LogState);
                        LogHelper.WriteLog("失败 tag:" + assetCurrenciesModel.data[0].tag, LogState);

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(" TradeOrder:" + ex.Message, LogState);
            }
            //});
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
            myConfig.TOs = new string[] { "xx" };
            myConfig.CCs = new string[] { };
            myConfig.From = "xx";
            myConfig.FromDisplayName = "xx";
            myConfig.Priority = System.Net.Mail.MailPriority.Normal;
            myConfig.Subject = Subject;

            EmailContent myContent = new EmailContent();
            myContent.Content = EmailContent;

            mailMan.SendMail(myConfig, myContent);
        }

        #endregion

        #region 属性

        private BindableCollection<RiseFallEntityModel> riseEntityModels = new BindableCollection<RiseFallEntityModel>();
        public BindableCollection<RiseFallEntityModel> RiseEntityModels
        {
            get { return riseEntityModels; }
            set { Set(ref riseEntityModels, value); }
        }

        private RiseFallEntityModel riseEntityModel = new RiseFallEntityModel();
        public RiseFallEntityModel RiseEntityModel
        {
            get { return riseEntityModel; }
            set { Set(ref riseEntityModel, value); }
        }

        private BindableCollection<RiseFallEntityModel> fallEntityModels = new BindableCollection<RiseFallEntityModel>();
        public BindableCollection<RiseFallEntityModel> FallEntityModels
        {
            get { return fallEntityModels; }
            set { Set(ref fallEntityModels, value); }
        }

        private RiseFallEntityModel fallEntityModel = new RiseFallEntityModel();
        public RiseFallEntityModel FallEntityModel
        {
            get { return fallEntityModel; }
            set { Set(ref fallEntityModel, value); }
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

        private Visibility riseVisibilitys = Visibility.Collapsed;
        public Visibility RiseVisibilitys
        {
            get { return riseVisibilitys; }
            set { Set(ref riseVisibilitys, value); }
        }

        private Visibility fallVisibilitys = Visibility.Collapsed;
        public Visibility FallVisibilitys
        {
            get { return fallVisibilitys; }
            set { Set(ref fallVisibilitys, value); }
        }

        private string fallAmplitude;
        public string FallAmplitude
        {
            get { return fallAmplitude; }
            set { Set(ref fallAmplitude, value); }
        }

        private string riseAmplitude;
        public string RiseAmplitude
        {
            get { return riseAmplitude; }
            set { Set(ref riseAmplitude, value); }
        }

        private double fallPercentage;
        public double FallPercentage
        {
            get { return fallPercentage; }
            set { Set(ref fallPercentage, value); }
        }

        private double risePercentage;
        public double RisePercentage
        {
            get { return risePercentage; }
            set { Set(ref risePercentage, value); }
        }

        private Visibility maxcontentvisibility;
        public Visibility MaxContentVisibility
        {
            get { return maxcontentvisibility; }
            set { Set(ref maxcontentvisibility, value); }
        }

        private Visibility minicontentvisibility;
        public Visibility MiniContentVisibility
        {
            get { return minicontentvisibility; }
            set { Set(ref minicontentvisibility, value); }
        }

        private bool logstate;
        public bool LogState
        {
            get { return logstate; }
            set { Set(ref logstate, value); }
        }

        #endregion

        #region Music

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
        /// <param name="hwndCallback">回调窗口的句柄，一般为NULL若成功则返回0，否则返回错误码。</param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string command, string returnString, int returnSize, IntPtr hwndCallback);

        public void Play()
        {
            mciSendString(@"close temp_alias", null, 0, 0);
            mciSendString(@"open ""E:/Music/青花瓷.mp3"" alias temp_alias", null, 0, 0);
            mciSendString("play temp_alias repeat", null, 0, 0);
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

        #endregion

        #region  获取USDT

        private BindableCollection<AccountBalanceEntityModel> accountBalanceEntityModels = new BindableCollection<AccountBalanceEntityModel>();
        public BindableCollection<AccountBalanceEntityModel> AccountBalanceEntityModels
        {
            get { return accountBalanceEntityModels; }
            set { Set(ref accountBalanceEntityModels, value); }
        }

        public async void GetAccountBalanceData()
        {
            try
            {
                AccountApi accountApi = new AccountApi(apiKey, secret, passPhrase);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls;
                Dictionary<string, string> requestParams = new Dictionary<string, string>();
                Task<string> task = accountApi.GetBalance(requestParams);
                string result = await task;
                AccountBalanceModel accountBalanceModel = JsonConvert.DeserializeObject<AccountBalanceModel>(result);

                if (accountBalanceModel.data == null)
                {
                    return;
                }
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

                    //if (Convert.ToDouble(accountBalanceEntityModel.availBal)>0.1)
                    //{
                    //    availBal = Convert.ToDouble(accountBalanceEntityModel.availBal);
                    //}
                    if (!string.IsNullOrEmpty(accountBalanceEntityModel.availEq))
                    {
                        if (Convert.ToDouble(accountBalanceEntityModel.availEq) > 0.1)
                        {
                            availEq = Convert.ToDouble(accountBalanceEntityModel.availEq);
                            ccy = accountBalanceModel.data[0].details[i].ccy;
                        }
                    }




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
                        //accountBalanceEntityModel.coinPath = GetImageDisplay(path);
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
            catch (Exception ex)
            {
                LogHelper.WriteLog("->GetAccountBalanceData->" + ex.Message, LogState);
            }
        }

        #endregion

        #region



        #endregion
    }
}
