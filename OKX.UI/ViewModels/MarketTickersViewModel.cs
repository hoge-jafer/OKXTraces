using Caliburn.Micro;
using Newtonsoft.Json;
using OKX.UI.Models;
using OKX.UI.Models.Entitys.MarketTickers;
using OKXSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.ViewModels
{
   public class MarketTickersViewModel : Screen
    {
        public MarketTickersViewModel()
        {
            GetData();
        }

        private string apiKey = "4d1613a9-2e14-4d23-be76-e8c3d9416d6f";
        private string secret = "E6A8DE3C92672CA71642A6AE7B95CDCF";
        private string passPhrase = "Zxcv123456";

        private BindableCollection<MarketTickersEntityModel> marketTickersEntityModels = new BindableCollection<MarketTickersEntityModel>();
        public BindableCollection<MarketTickersEntityModel> MarketTickersEntityModels
        {
            get { return marketTickersEntityModels; }
            set { Set(ref marketTickersEntityModels, value); }
        }

        public async void GetData()
        {
            MarketDataApi marketDataApi = new MarketDataApi(apiKey, secret, passPhrase);
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            requestParams["instType"] = "SPOT";
            Task<string> task = marketDataApi.GetTickers(requestParams);
            string result = await task;
            MarketTickersModel marketTickersModel = JsonConvert.DeserializeObject<MarketTickersModel>(result);
        }
    }
}
