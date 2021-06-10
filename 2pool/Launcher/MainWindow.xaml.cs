using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int version;
        private HttpClient _httpClient;
        private WebClient webClient;
        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists("log.2pool"))
            {
                File.Create("log.2pool");
            }
            var json = File.ReadAllText("version.json");
            VersionUpdate v = JsonSerializer.Deserialize<VersionUpdate>(json);
            version = v.Version;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:8000");
            var dueTime = TimeSpan.FromSeconds(3);
            var interval = TimeSpan.FromSeconds(10);
            RunPeriodicAsync(CheckUpdate, dueTime, interval, CancellationToken.None);
        }
        
        private static async Task RunPeriodicAsync(Action onTick,
            TimeSpan dueTime, 
            TimeSpan interval, 
            CancellationToken token)
        {
            // Initial wait time before we begin the periodic loop.
            if(dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token);

            // Repeat this loop until cancelled.
            while(!token.IsCancellationRequested)
            {
                // Call our onTick function.
                onTick?.Invoke();

                // Wait to repeat again.
                if(interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);       
            }
        }

        private async Task DownloadUpdate()
        {
            try
            {
                webClient = new WebClient();
                webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadFileAsync(new Uri("https://hls.smartlms.vn/media/2021/06/09/fb/5e75eb186ddb5e7b88596ffb/60bfa498453b2b13a31f1f55.zip"),
                    version+".zip");
            }
            catch (Exception e)
            {
                File.AppendText("\n"+"["+DateTime.Now+"] "+ e.Message);
            }
            
        }

        private void Completed(object sender, AsyncCompletedEventArgs e) {
            webClient = null;
            try
            {
                ZipFile.ExtractToDirectory(version+".zip", AppContext.BaseDirectory, Encoding.UTF8, true);
                File.Delete(version+".zip");
            }
            catch (Exception exception)
            {
                File.AppendText("\n"+"["+DateTime.Now+"] "+ exception.Message);
            }
        }
        
        public async void CheckUpdate()
        {
            try
            {
                var res = await _httpClient.GetAsync("/check-update");
                res.EnsureSuccessStatusCode();
                var resultStr = await res.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<VersionData>(resultStr);
                if (result.version > version)
                {
                    version = result.version;
                    DownloadUpdate();
                }
            }
            catch (Exception e)
            {
                File.AppendText("\n"+"["+DateTime.Now+"] "+ e.Message);
            }
        }
    }

    class VersionData
    {
        public string status { get; set; }
        public int version { get; set; }
    }
}