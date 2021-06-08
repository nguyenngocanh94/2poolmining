using System;
using System.Timers;
using Caliburn.Micro;
using Chia2Pool.Chia;
using Chia2Pool.Common;
using Chia2Pool.Pool;
using ChiaRpc.Exceptions;

namespace Chia2Pool.Periodical
{
    public class SetRewardPeriodical : BaseTimer
    {
        
        protected readonly IEventAggregator Events;
        public IEventAggregator EventAggregator => Events;
        
        private ChiaController _chiaController;
        private PoolClient _poolClient;
        private string _poolAddress;
        
        public SetRewardPeriodical(int time, string initialAddress, IEventAggregator events) : base(time)
        {
            _chiaController = new ChiaController(Settings.GetInstance().SslDirectory);
            _poolAddress = initialAddress;
            this.Events = events;
            _poolClient = new PoolClient(Settings.GetInstance().PoolUrl, Settings.GetInstance().ApiKey);
        }

        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var data  = _poolClient.GetPoolInfo();
                _poolAddress = data.Data.Pa;
                _chiaController.FarmerClient.SetRewardTargets(_poolAddress);
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

        protected void OnStop(string message)
        {
            // show log
        }
    }
}