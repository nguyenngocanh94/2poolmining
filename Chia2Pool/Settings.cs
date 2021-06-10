using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Chia2Pool
{
    public class Settings
         {
             public string? ApiKey { get; set; }
             public string? SslDirectory { get; set; }
             public string? PoolUrl { get; set; }
             public string? FarmerTarget { get; set; }
             public string? PoolTarget { get; set; }
             
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

             public async Task Persist()
             {
                 var setting = new SettingTemp()
                 {
                     ApiKey = ApiKey,
                     SslDirectory = SslDirectory,
                     PoolUrl = PoolUrl,
                     FarmerTarget = FarmerTarget,
                     PoolTarget = PoolTarget,
                 };

                 var stringSetting = JsonConvert.SerializeObject(setting);
                 await File.WriteAllTextAsync("keys.2pool",stringSetting, CancellationToken.None);
             }

             public void Fetch()
             {
                 string runtimeDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                 if (File.Exists(runtimeDirectory + "\\keys.2pool"))
                 {
                     var self = JsonSerializer.Deserialize<SettingTemp>(File.ReadAllText(runtimeDirectory + "/keys.2pool"));
                     _instance.ApiKey = self.ApiKey;
                     _instance.SslDirectory = self.SslDirectory;
                     _instance.PoolUrl = self.PoolUrl;
                 }
             }
         }
     
         public class SettingTemp
         {
             public string ApiKey { get; set; }
             public string SslDirectory { get; set; }
             public string PoolUrl { get; set; }
             
             public string FarmerTarget { get; set; }
             public string PoolTarget { get; set; }
         }
}