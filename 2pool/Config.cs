using System.IO;
using System.Text.Json;

namespace _2pool
{
    public class Config
    {
        public string DefaultChiaExe { get; set; }
        private static readonly Config instance = new Config();

        private Config()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string json = File.ReadAllText(currentDir+"/appsettings.json");
            
            var self = JsonSerializer.Deserialize<ConfigDto>(json);
            DefaultChiaExe = self.DefaultChiaExe;
        }

        public static Config Instance()
        {
            return instance;
        }
    }

    public class ConfigDto
    {
        public string DefaultChiaExe { get; set; }
    }
}