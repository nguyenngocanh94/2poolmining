using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Chia2Pool.Common;
using Chia2Pool.Encrypt;

namespace Chia2Pool.Pool
{
    public class PoolClient
    {
        private HttpClient _client;
        private string baseUrl;
        private string appKey;

        public PoolClient(string baseUrl, string appKey)
        {
            _client = new HttpClient();
            this.baseUrl = baseUrl;
            this.appKey = appKey;
        }

        public P GetPoolInfo()
        {
            return Post<P>(new Uri(baseUrl + "/m"),
                ClsCrypto.EncryptStringAes(JsonSerializer.Serialize(MessageFactory.CreatePir())));
        }

        public void SendData(SendInfo data)
        {
            PostAsync<B>(new Uri(baseUrl + "/m"), ClsCrypto.EncryptStringAes(JsonSerializer.Serialize(data)));
        }

        protected T Post<T>(Uri requestUri, string encrypted) where T : B
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(encrypted)
            };
            request.Headers.Add("apikey", appKey);
            var response = _client.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var resultRes = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var real = ClsCrypto.DecryptStringAes(resultRes);
            var result = JsonSerializer.Deserialize<T>(real); 
            return result.Status != "success"
                ? throw new PoolHttpException("pool response with unsuccessful")
                : result;
        }

        protected async Task<T> PostAsync<T>(Uri requestUri, string encrypted) where T : B
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(encrypted)
            };
            request.Headers.Add("apikey", appKey);
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var resultStr = await response.Content.ReadAsStringAsync();
            var real = ClsCrypto.DecryptStringAes(resultStr);
            var result = JsonSerializer.Deserialize<T>(real); 
            return result.Status != "success"
                ? throw new PoolHttpException("pool response with unsuccessful")
                : result;
        }
    }

    public class B
    {
        public string Status { get; set; }
    }

    public class P : B
    {
        public D Data { get; set; }
    }

    public class D
    {
        public string Pa { get; set; }
    }

    public class SendInfo
    {
        public string T { get; set; }
        public Data D { get; set; }
        public string N { get; set; }
    }

    public class Data
    {
        public PlotDetail[] P { get; set; }
    }

    public class PlotDetail
    {
        public int K { get; set; }
        public int N { get; set; }
        public ulong S { get; set; }
    }

    public class MessageFactory
    {
        public static PoolInfoRequest CreatePir()
        {
            return new()
            {
                T = "PI",
                N = RandomUtil.GetSixRandom()
            };
        }
    }

    public class PoolInfoRequest
    {
        public string T { get; set; }
        public string N { get; set; }
    }
}