using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var se = new SettingTemp();
            se.ApiKey = "11a607db-8275-47c0-ab61-b8c18a5dde5c";
            se.SslDirectory = @"C:\Users\Admin\.chia\mainnet\config\ssl";
            se.PoolUrl = "https://test-net.mining-hub.2pool.io";
            
            Console.WriteLine(JsonConvert.SerializeObject(se));
        }
        
        public class Settings
        {
            public string ApiKey { get; set; }
            public string SslDirectory { get; set; }
            public string PoolUrl { get; set; }
        
            private Settings()
            {
            
            }
        
            private static Settings _instance;
        
            public static Settings GetInstance()
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                }

                return _instance;
            }
        }

        public class SettingTemp
        {
            public string ApiKey { get; set; }
            public string SslDirectory { get; set; }
            public string PoolUrl { get; set; }
        }
    }
}
