using System;
using System.Threading.Tasks;
using System.Timers;
using Caliburn.Micro;
using Chia2Pool.Chia;
using Chia2Pool.Common;
using Chia2Pool.Pool;
using ChiaRpc.Exceptions;

namespace Chia2Pool.Periodical
{
    public class SetRewardPeriodical
    {
        private System.Timers.Timer timer;
        protected readonly IEventAggregator Events;
        public IEventAggregator EventAggregator => Events;
        
        private ChiaController _chiaController;
        private PoolClient _poolClient;
        private string _poolAddress;
        
        public SetRewardPeriodical(IEventAggregator events)
        {
            _chiaController = new ChiaController(Settings.GetInstance().SslDirectory);
            this.Events = events;
            _poolClient = new PoolClient(Settings.GetInstance().PoolUrl, Settings.GetInstance().ApiKey);
        }

        public async Task Run()
        {
            try
            {
                Events.PublishOnUIThreadAsync(Logger.Info("Get Plot Info .. "));
                var data  = _poolClient.GetPoolInfo();
                _poolAddress = data.data.pa;
                await Events.PublishOnUIThreadAsync(Logger.Info("Set Reward target .. "));
                await _chiaController.FarmerClient.SetRewardTargets(_poolAddress);
            }
            catch (Exception exception)
            {
                if (exception is ChiaHttpException)
                {
                    Events.PublishOnUIThreadAsync(Logger.Warn(exception.Message));
                }

                if (exception is PoolHttpException)
                {
                    Events.PublishOnUIThreadAsync(Logger.Warn(exception.Message));
                }
            }
        }
    }
}