using Caliburn.Micro;
using OKXSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.ViewModels
{
    public class AccountPositionsViewModel : Screen
    {
        public AccountPositionsViewModel()
        {

        }

        private string apiKey = "4d1613a9-2e14-4d23-be76-e8c3d9416d6f";
        private string secret = "E6A8DE3C92672CA71642A6AE7B95CDCF";
        private string passPhrase = "Zxcv123456";

        public async void GetData()
        {
            AccountApi accountApi = new AccountApi(apiKey, secret, passPhrase);
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            Task<string> task = accountApi.GetPositions(requestParams);
            string result = await task;
        }
    }
}
