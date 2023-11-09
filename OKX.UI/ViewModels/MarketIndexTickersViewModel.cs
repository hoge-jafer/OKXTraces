using Caliburn.Micro;
using Newtonsoft.Json;
using OKX.UI.Models;
using OKXSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.ViewModels
{
   public class MarketIndexTickersViewModel : Screen
    {
        public MarketIndexTickersViewModel()
        {

        }

        private string apiKey = "";
        private string secret = "";
        private string passPhrase = "";

        public async void GetData()
        {
            //quoteCcy	String	可选	指数计价单位， 目前只有 USD/USDT/BTC为计价单位的指数，quoteCcy和instId必须填写一个
            //instId String  可选 指数，如 BTC-USD
            MarketDataApi marketDataApi = new MarketDataApi(apiKey, secret, passPhrase);
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            //requestParams["quoteCcy"] = "LINK";
            requestParams["instId"] = "LINK-USD";
            Task<string> task = marketDataApi.GetIndexTickers(requestParams);
            string result = await task;
            MarketIndexTickersModel marketIndexTickersModel = JsonConvert.DeserializeObject<MarketIndexTickersModel>(result);
        }
    }
}
