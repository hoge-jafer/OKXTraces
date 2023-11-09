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

        private string apiKey = "";
        private string secret = "";
        private string passPhrase = "";

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
