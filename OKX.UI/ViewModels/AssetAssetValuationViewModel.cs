using Caliburn.Micro;
using Newtonsoft.Json;
using OKX.UI.Models;
using OKX.UI.Models.Entitys.AssetAssetValuation;
using OKXSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.ViewModels
{
   public class AssetAssetValuationViewModel : Screen
    {
        public AssetAssetValuationViewModel()
        {
            GetData();
        }
        private string apiKey = "4d1613a9-2e14-4d23-be76-e8c3d9416d6f";
        private string secret = "E6A8DE3C92672CA71642A6AE7B95CDCF";
        private string passPhrase = "Zxcv123456";

        private BindableCollection<AssetAssetValuationEntityModel> assetAssetValuationEntityModels = new BindableCollection<AssetAssetValuationEntityModel>();
        public BindableCollection<AssetAssetValuationEntityModel> AssetAssetValuationEntityModels
        {
            get { return assetAssetValuationEntityModels; }
            set { Set(ref assetAssetValuationEntityModels, value); }
        }

        public async void GetData()
        {
            FundingApi fundingApi = new FundingApi(apiKey, secret, passPhrase);
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            requestParams["ccy"] = "USDT";
            Task<string> task = fundingApi.AssetValuation(requestParams);
            string result = await task;
            AssetAssetValuationModel assetAssetValuationModel = JsonConvert.DeserializeObject<AssetAssetValuationModel>(result);

            for (int i = 0; i < assetAssetValuationModel.data.Length; i++)
            {
                AssetAssetValuationEntityModel assetAssetValuationEntityModel = new AssetAssetValuationEntityModel();
                assetAssetValuationEntityModel.classic = assetAssetValuationModel.data[i].details.classic;
                assetAssetValuationEntityModel .earn= assetAssetValuationModel.data[i].details.earn;
                assetAssetValuationEntityModel.funding = assetAssetValuationModel.data[i].details.funding;
                assetAssetValuationEntityModel.trading = assetAssetValuationModel.data[i].details.trading;
                AssetAssetValuationEntityModels.Add(assetAssetValuationEntityModel);
            }

        }
    }
}
