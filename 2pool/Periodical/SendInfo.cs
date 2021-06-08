using System;
using System.Threading.Tasks;
using System.Timers;
using _2pool.Chia;
using Timer;

namespace _2pool.Periodical
{
    public class SendInfo : BaseTimer
    {
        
        private ChiaController _chiaController;
        private PoolClient _poolClient;
        
        public SendInfo(int time) : base(time)
        {
            _chiaController = new ChiaController("C://Users/Admin//.chia//mainnet//config//ssl");
            _poolClient = new PoolClient("https://test-net.mining-hub.2pool.io", "11a607db-8275-47c0-ab61-b8c18a5dde5c");
        }
        
        protected void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(async () =>
            {
                var plots = _chiaController.HarvesterClient.GetPlotDirectoriesAsync();
            });
        }

        protected void OnStop()
        {
            // publish message to view
        }
    }
}