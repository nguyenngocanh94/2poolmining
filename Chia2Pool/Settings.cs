using System.ComponentModel.Composition;

namespace Chia2Pool
{
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