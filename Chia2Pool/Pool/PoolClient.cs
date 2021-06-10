using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Chia2Pool.Common;
using Chia2Pool.Encrypt;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
            var result = JsonSerializer.Deserialize<T>(resultRes); 
            return result.status != "success"
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
            var result = JsonSerializer.Deserialize<T>(resultStr); 
            return result.status != "success"
                ? throw new PoolHttpException("pool response with unsuccessful")
                : result;
        }
    }

    public class B
    {
        public string status { get; set; }
    }

    public class P : B
    {
        public D data { get; set; }
    }

    public class D
    {
        public string pa { get; set; }
    }

    public class SendInfo
    {
        public string t { get; set; }
        public Data d { get; set; }
        public string n { get; set; }
    }

    public class Data
    {
        public PlotDetail[] p { get; set; }
    }

    public class PlotDetail
    {
        public int k { get; set; }
        public int n { get; set; }
        public ulong s { get; set; }
    }

    public class MessageFactory
    {
        public static PoolInfoRequest CreatePir()
        {
            return new()
            {
                t = "PI",
                n = RandomUtil.GetSixRandom()
            };
        }
    }

    public class PoolInfoRequest
    {
        public string t { get; set; }
        public string n { get; set; }
    }
}