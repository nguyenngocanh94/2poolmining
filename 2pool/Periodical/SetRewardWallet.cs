using System;
using System.Threading.Tasks;
using System.Timers;
using _2pool.Chia;
using Timer;

namespace _2pool.Periodical
{
    public class SetRewardWallet : BaseTimer
    {
        private ChiaController _chiaController;
        private PoolClient _poolClient;
        private string poolAddress;
        public SetRewardWallet(int time, string address) : base(time)
        {
            _chiaController = new ChiaController("C://Users/Admin//.chia//mainnet//config//ssl");
            poolAddress = address;
            _poolClient = new PoolClient("https://test-net.mining-hub.2pool.io", "11a607db-8275-47c0-ab61-b8c18a5dde5c");
        }
        
        protected void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var data  = _poolClient.GetPoolInfo();
                poolAddress = data.Data.Pa;
                _chiaController.FarmerClient.SetRewardTargets(poolAddress);
            }
            catch (Exception exception)
            {
               this.OnStop();
            }
        }

        protected void OnStop()
        {
            // publish message to view
        }
    }
}