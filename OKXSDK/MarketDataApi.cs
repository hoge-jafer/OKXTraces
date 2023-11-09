using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OKXSDK
{
    public class MarketDataApi : SdkApi
    {
        private string MARKET_SEGMENT = "api/v5/market";
        public MarketDataApi(string apikey, string secret, string passPhrase) : base(apikey, secret, passPhrase) { }

        public async Task<string> GetTickers(Dictionary<string, string> requestParams)
        {
            var url = $"{this.BASEURL}{this.MARKET_SEGMENT}/tickers";
            using (var client = new HttpClient(new HttpInterceptor(this._apiKey, this._secret, this._passPhrase, null)))
            {
                var res = await client.GetAsync(await createGetUrl(url, requestParams));
                var contentStr = await res.Content.ReadAsStringAsync();
                return contentStr;
            }
        }


        public async Task<string> GetTicker(Dictionary<string, string> requestParams)
        {
            #region

            //using (var httpClientHandler = new HttpClientHandler())
            //{
            //    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            //    using (var client = new HttpClient(httpClientHandler))
            //    {             
            //        // Make your request...
            //    }
            //}

            #endregion

            var url = $"{this.BASEURL}{this.MARKET_SEGMENT}/ticker";
            //using (var client = new HttpClient(new HttpInterceptor(this._apiKey, this._secret, this._passPhrase, null)))
            //{
            //    var res = await client.GetAsync(await createGetUrl(url, requestParams));
            //    var contentStr = await res.Content.ReadAsStringAsync();
            //    return contentStr;
            //}

            using (HttpClient client = new HttpClient(new HttpInterceptor(this._apiKey, this._secret, this._passPhrase, null)))
            {
                var result = await client.GetAsync(await createGetUrl(url, requestParams));
                if (result.IsSuccessStatusCode)
                {
                    string str = await result.Content.ReadAsStringAsync();
                    return str;
                }
                else
                {
                    return "";
                }
            }

            //using (var httpClientHandler = new HttpClientHandler())
            //{
            //    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            //    using (var client = new HttpClient(httpClientHandler))
            //    {
            //        // Make your request...
            //    }
            //}
        }


        public async Task<string> GetIndexTickers(Dictionary<string, string> requestParams)
        {
            var url = $"{this.BASEURL}{this.MARKET_SEGMENT}/index-tickers";
            using (var client = new HttpClient(new HttpInterceptor(this._apiKey, this._secret, this._passPhrase, null)))
            {
                var res = await client.GetAsync(await createGetUrl(url, requestParams));
                var contentStr = await res.Content.ReadAsStringAsync();
                return contentStr;
            }
        }


        public async Task<string> GetOrderBook(Dictionary<string, string> requestParams)
        {
            var url = $"{this.BASEURL}{this.MARKET_SEGMENT}/books";
            using (var client = new HttpClient(new HttpInterceptor(this._apiKey, this._secret, this._passPhrase, null)))
            {
                var res = await client.GetAsync(await createGetUrl(url, requestParams));
                var contentStr = await res.Content.ReadAsStringAsync();
                return contentStr;
            }
        }


        public async Task<string> GetCandlesticks(Dictionary<string, string> requestParams)
        {
            var url = $"{this.BASEURL}{this.MARKET_SEGMENT}/candles";
            using (var client = new HttpClient(new HttpInterceptor(this._apiKey, this._secret, this._passPhrase, null)))
            {
                string _u = await createGetUrl(url, requestParams);
                var res = await client.GetAsync(_u);
                var contentStr = await res.Content.ReadAsStringAsync();
                return contentStr;
            }
        }


        public async Task<string> GetCandlesticksHistory(Dictionary<string, string> requestParams)
        {
            var url = $"{this.BASEURL}{this.MARKET_SEGMENT}/history-candles";
            using (var client = new HttpClient(new HttpInterceptor(this._apiKey, this._secret, this._passPhrase, null)))
            {
                var res = await client.GetAsync(await createGetUrl(url, requestParams));
                var contentStr = await res.Content.ReadAsStringAsync();
                return contentStr;
            }
        }


        public async Task<string> GetIndexCandlesticks(Dictionary<string, string> requestParams)
        {
            var url = $"{this.BASEURL}{this.MARKET_SEGMENT}/index-candles";
            using (var client = new HttpClient(new HttpInterceptor(this._apiKey, this._secret, this._passPhrase, null)))
            {
                var res = await client.GetAsync(await createGetUrl(url, requestParams));
                var contentStr = await res.Content.ReadAsStringAsync();
                return contentStr;
            }
        }


        public async Task<string> GetMarkPriceCandlesticks(Dictionary<string, string> requestParams)
        {
            var url = $"{this.BASEURL}{this.MARKET_SEGMENT}/mark-price-candles";
            using (var client = new HttpClient(new HttpInterceptor(this._apiKey, this._secret, this._passPhrase, null)))
            {
                var res = await client.GetAsync(await createGetUrl(url, requestParams));
                var contentStr = await res.Content.ReadAsStringAsync();
                return contentStr;
            }
        }


        public async Task<string> GetTrades(Dictionary<string, string> requestParams)
        {
            var url = $"{this.BASEURL}{this.MARKET_SEGMENT}/trades";
            using (var client = new HttpClient(new HttpInterceptor(this._apiKey, this._secret, this._passPhrase, null)))
            {
                var res = await client.GetAsync(await createGetUrl(url, requestParams));
                var contentStr = await res.Content.ReadAsStringAsync();
                return contentStr;
            }
        }

    }
}
