using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OKXSDK
{
    public class HttpInterceptor : DelegatingHandler
    {
        private string _apiKey;
        private string _passPhrase;
        private string _secret;
        private string _bodyStr;
        public HttpInterceptor(string apiKey, string secret, string passPhrase, string bodyStr)
        {
            this._apiKey = apiKey;
            this._passPhrase = passPhrase;
            this._secret = secret;
            this._bodyStr = bodyStr;
            //InnerHandler = new HttpClientHandler();

            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
            };
            ////using (HttpClient client = new HttpClient(httpClientHandler))
            ////{
            ////    string url = WeiXinSettings.GetJscode2Session(code);
            ////    var result = await client.GetAsync(url);
            ////    if (result.IsSuccessStatusCode)
            ////    {
            ////        string str = await result.Content.ReadAsStringAsync();
            ////        return str;
            ////    }
            ////}
            InnerHandler = httpClientHandler;

            ////var clientHandler = new HttpClientHandler();
            ////clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            ////clientHandler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls; //SecurityProtocolType.Tls12 | SecurityProtocolType.Tls;
            ////InnerHandler = clientHandler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var method = request.Method.Method;
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("OK-ACCESS-KEY", this._apiKey);

            var now = DateTime.Now;
            var timeStamp = TimeZoneInfo.ConvertTimeToUtc(now).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var requestUrl = request.RequestUri.PathAndQuery;
            string sign = "";
            if (!String.IsNullOrEmpty(this._bodyStr))
            {
                sign = Encryptor.HmacSHA256($"{timeStamp}{method}{requestUrl}{this._bodyStr}", this._secret);
            }
            else
            {
                sign = Encryptor.HmacSHA256($"{timeStamp}{method}{requestUrl}", this._secret);
            }

            request.Headers.Add("OK-ACCESS-SIGN", sign);
            request.Headers.Add("OK-ACCESS-TIMESTAMP", timeStamp.ToString());
            request.Headers.Add("OK-ACCESS-PASSPHRASE", this._passPhrase);
            //request.Headers.Add("x-simulated-trading", "1");
            request.Headers.Add("x-simulated-trading", "0");

            return base.SendAsync(request, cancellationToken);
        }
    }
}
