using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Chia.NET.Models;

namespace _2pool
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
            return Post<P>(new Uri(baseUrl+"/m"), new Dictionary<string, string>(){["t"] = "PI"});
        }

        public void SendData(SendInfo data)
        {
            PostAsync<B>(new Uri(baseUrl+"/m"), data);
        }
        
        protected T Post<T>(Uri requestUri, IDictionary<string, string> parameters = null) where T : B
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = JsonContent.Create(parameters ?? new Dictionary<string, string>()),
                
            };
            request.Headers.Add("apikey", appKey);
            var response = _client.Send(request);
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadFromJsonAsync<T>().GetAwaiter().GetResult();

            return result.Status != "success"
                ? throw new HttpRequestException("Chia responded with unsuccessful")
                : result;
        }
        
        protected async Task<T> PostAsync<T>(Uri requestUri, object json) where T : B
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = JsonContent.Create(json)
            };
            request.Headers.Add("apikey", appKey);
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<T>();

            return result.Status != "success"
                ? throw new HttpRequestException("Chia responded with unsuccessful")
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
        public string D { get; set; }
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
        public string S { get; set; }
    }
}