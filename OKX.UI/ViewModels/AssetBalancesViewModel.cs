using Caliburn.Micro;
using Newtonsoft.Json;
using OKX.UI.Models;
using OKX.UI.Models.Entitys.AssetBalances;
using OKXSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.ViewModels
{
    public class AssetBalancesViewModel : Screen
    {
        public AssetBalancesViewModel()
        {
            GetAccountBalancesData();
        }
        private string apiKey = "";
        private string secret = "";
        private string passPhrase = "";

        #region

        private BindableCollection<AssetBalancesEntityModel> assetBalancesEntityModels = new BindableCollection<AssetBalancesEntityModel>();
        public BindableCollection<AssetBalancesEntityModel> AssetBalancesEntityModels
        {
            get { return assetBalancesEntityModels; }
            set { Set(ref assetBalancesEntityModels, value); }
        }

        #endregion
        public async void GetAccountBalancesData()
        {
            FundingApi fundingApi = new FundingApi(apiKey, secret, passPhrase);
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            Task<string> task = fundingApi.GetBalance(requestParams);
            string result = await task;
            AssetBalancesModel assetBalancesModel = JsonConvert.DeserializeObject<AssetBalancesModel>(result);
            for (int i = 0; i < assetBalancesModel.data.Length; i++)
            {
                AssetBalancesEntityModel accountBalanceEntityModel = new AssetBalancesEntityModel();
                /// <summary>
                /// 可用余额
                /// </summary>
                accountBalanceEntityModel.availBal = assetBalancesModel.data[i].availBal;

                /// <summary>
                /// 余额
                /// </summary>
                accountBalanceEntityModel.bal = assetBalancesModel.data[i].bal;

                /// <summary>
                /// 币种
                /// </summary>
                accountBalanceEntityModel.ccy = assetBalancesModel.data[i].ccy;

                /// <summary>
                /// 冻结（不可用）
                /// </summary>
                accountBalanceEntityModel.frozenBal = assetBalancesModel.data[i].frozenBal;
                AssetBalancesEntityModels.Add(accountBalanceEntityModel);
            }

        }
    }


}
