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

                //监听BTC
                //MonitorBitcoin();
                //MonitorMarket();
                Thread.Sleep(110);
            }
        }

        #endregion

        #region Command



        #endregion


    }
}
