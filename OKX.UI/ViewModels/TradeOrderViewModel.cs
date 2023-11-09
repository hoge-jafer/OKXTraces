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
        private string apiKey = "";
        private string secret = "";
        private string passPhrase = "";
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
