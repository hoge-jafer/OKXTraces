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

        private string apiKey = "";
        private string secret = "";
        private string passPhrase = "";

        public async void GetData()
        {
            AccountApi accountApi = new AccountApi(apiKey, secret, passPhrase);
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            Task<string> task = accountApi.GetPositions(requestParams);
            string result = await task;
        }
    }
}
