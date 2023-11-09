using Caliburn.Micro;
using Newtonsoft.Json;
using OKX.UI.Models;
using OKX.UI.Models.Entitys.AssetCurrencies;
using OKXSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.ViewModels
{
    public class AssetCurrenciesViewModel : Screen
    {
        public AssetCurrenciesViewModel()
        {
            GetData();
        }

        private string apiKey = "";
        private string secret = "";
        private string passPhrase = "";

        private BindableCollection<AssetCurrenciesEntityModel> assetCurrenciesEntityModels = new BindableCollection<AssetCurrenciesEntityModel>();
        public BindableCollection<AssetCurrenciesEntityModel> AssetCurrenciesEntityModels
        {
            get { return assetCurrenciesEntityModels; }
            set { Set(ref assetCurrenciesEntityModels, value); }
        }

        public async void GetData()
        {
            //6 次/s
            FundingApi fundingApi = new FundingApi(apiKey, secret, passPhrase);
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            Task<string> task = fundingApi.GetCurrencies(requestParams);
            string result = await task;
            AssetCurrenciesModel assetCurrenciesModel = JsonConvert.DeserializeObject<AssetCurrenciesModel>(result);
            for (int i = 0; i < assetCurrenciesModel.data.Length; i++)
            {
                AssetCurrenciesEntityModel assetCurrenciesEntityModel = new AssetCurrenciesEntityModel();
                /// <summary>
                /// 是否可充值，false表示不可链上充值，true表示可以链上充值
                /// </summary>
                assetCurrenciesEntityModel.canDep = assetCurrenciesModel.data[i].canDep;

                /// <summary>
                /// 是否可内部转账，false表示不可内部转账，true表示可以内部转账
                /// </summary>
                assetCurrenciesEntityModel.canInternal = assetCurrenciesModel.data[i].canInternal;

                /// <summary>
                /// 是否可提币，false表示不可链上提币，true表示可以链上提币
                /// </summary>
                assetCurrenciesEntityModel.canWd = assetCurrenciesModel.data[i].canWd;

                /// <summary>
                /// 币种名称
                /// </summary>
                assetCurrenciesEntityModel.ccy = assetCurrenciesModel.data[i].ccy;

                /// <summary>
                /// 币种链信息有的币种下有多个链，必须要做区分，如USDT下有USDT-ERC20，USDT-TRC20，USDT-Omni多个链
                /// </summary>
                assetCurrenciesEntityModel.chain = assetCurrenciesModel.data[i].chain;

                /// <summary>
                /// 当前链是否为主链如果是则返回true，否则返回false
                /// </summary>
                assetCurrenciesEntityModel.mainNet = assetCurrenciesModel.data[i].mainNet;

                /// <summary>
                /// 最大提币手续费数量
                /// </summary>
                assetCurrenciesEntityModel.maxFee = assetCurrenciesModel.data[i].maxFee;

                /// <summary>
                /// 最小提币手续费数量
                /// </summary>
                assetCurrenciesEntityModel.minFee = assetCurrenciesModel.data[i].minFee;

                /// <summary>
                /// 币种最小提币量
                /// </summary>
                assetCurrenciesEntityModel.minWd = assetCurrenciesModel.data[i].minWd;

                /// <summary>
                /// 币种中文名称，不显示则无对应名称
                /// </summary>
                assetCurrenciesEntityModel.name = assetCurrenciesModel.data[i].name;

                AssetCurrenciesEntityModels.Add(assetCurrenciesEntityModel);
            }
        }
    }
}
