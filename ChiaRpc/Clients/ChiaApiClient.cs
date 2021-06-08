using Chia.NET.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ChiaRpc.Exceptions;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Chia.NET.Clients
{
    public abstract class ChiaApiClient
    {
        private string SslDirectory;

        private HttpClient Client;
        private readonly string CertName;
        private readonly string ApiUrl;

        public ChiaApiClient(string ssl, string certName, string apiUrl)
        {
            CertName = certName;
            ApiUrl = apiUrl;
            SslDirectory = ssl;
        }

        protected Task InitializeAsync()
        {
            string certificatePath = Path.Combine(SslDirectory, CertName, $"private_{CertName}.crt");
            string keyPath = Path.Combine(SslDirectory, CertName, $"private_{CertName}.key");
            X509Certificate2 certWithKey = LoadPemCertificate(certificatePath, keyPath);
            var cert = new X509Certificate2(certWithKey.Export(X509ContentType.Pfx, "2pool"),"2pool");
            
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };
            handler.ClientCertificates.Add(cert);
            Client = new HttpClient(handler);

            return Task.CompletedTask;
        }
        
        public  X509Certificate2 LoadPemCertificate(string certificatePath, string privateKeyPath)
        {
            using var publicKey = new X509Certificate2(certificatePath);

            var privateKeyText = File.ReadAllText(privateKeyPath);
            var privateKeyBlocks = privateKeyText.Split("-", StringSplitOptions.RemoveEmptyEntries);
            var privateKeyBytes = Convert.FromBase64String(privateKeyBlocks[1]);
            using var rsa = RSA.Create();

            if (privateKeyBlocks[0] == "BEGIN PRIVATE KEY")
            {
                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
            }
            else if (privateKeyBlocks[0] == "BEGIN RSA PRIVATE KEY")
            {
                rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
            }

            return publicKey.CopyWithPrivateKey(rsa);
        }



        /// <summary>
        /// Returns a list of peers that we are currently connected to.
        /// </summary>
        /// <returns></returns>
        public async Task<ChiaConnection[]> GetConnections()
        {
            var result = await PostAsync<GetConnectionsResult>(SharedRoutes.GetConnections(ApiUrl));
            return result.Connections;
        }

        protected async Task<T> PostAsync<T>(Uri requestUri, IDictionary<string, string> parameters = null) where T : ChiaResult
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, 
                    "application/json")
            };
            

            var response = await Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var stringRes = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(stringRes);
            return !result.Success
                ? throw new ChiaHttpException("Chia responded with unsuccessful")
                : result;
        }

        protected Task PostAsync(Uri requestUri, IDictionary<string, string> parameters = null)
            => PostAsync<ChiaResult>(requestUri, parameters);
        
        
        public async Task<ChiaResult> StopNode()
        {
            var data = await PostAsync<ChiaResult>(SharedRoutes.StopNode(ApiUrl));
            Console.WriteLine(data.Success);
            return data;
        }
        
        protected T Post<T>(Uri requestUri, IDictionary<string, string> parameters = null) where T : ChiaResult
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(JsonSerializer.Serialize(parameters),
                    Encoding.UTF8, 
                    "application/json")
            };
            
            var response = Client.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var stringRes =  response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var result = JsonSerializer.Deserialize<T>(stringRes);

            return !result.Success
                ? throw new ChiaHttpException("Chia responded with unsuccessful")
                : result;
        }
    }
}
