using Caliburn.Micro;
using OKXSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.ViewModels
{
    public class TradeOrderViewModel : Screen
    {
        private string apiKey = "4d1613a9-2e14-4d23-be76-e8c3d9416d6f";
        private string secret = "E6A8DE3C92672CA71642A6AE7B95CDCF";
        private string passPhrase = "Zxcv123456";
        public TradeOrderViewModel()
        {
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls;
            Getdata();
        }

        public async void Getdata()
        {
            TradeApi tradeApi = new TradeApi(apiKey, secret, passPhrase);
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            Task<string> task = tradeApi.PlaceOrder(requestParams);
            string result = await task;
        }
    }
}
