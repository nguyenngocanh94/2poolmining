using System;
using System.Threading.Tasks;
using System.Timers;
using Caliburn.Micro;
using Chia2Pool.Chia;
using Chia2Pool.Common;
using Chia2Pool.Messages;
using Chia2Pool.Pool;
using ChiaRpc.Exceptions;

namespace Chia2Pool.Periodical
{
    public class SendInfoPeriodical
    {
        private ChiaController _chiaController;
        private PoolClient _poolClient;
        
        protected readonly IEventAggregator events;
        public IEventAggregator EventAggregator => events;
        
        
        public SendInfoPeriodical(IEventAggregator events)
        {
            this.events = events;
            _chiaController = new ChiaController(Settings.GetInstance().SslDirectory);
            _poolClient = new PoolClient(Settings.GetInstance().PoolUrl, Settings.GetInstance().ApiKey);
        }
        
        public async Task Run()
        {
            try
            {
                events.PublishOnUIThreadAsync(Logger.Info("Send plots to server .. "));
                var plots = await _chiaController.HarvesterClient.GetPlotsAsync();
                _poolClient.SendData(HandlePlots.HandlePlot(plots));
            }
            catch (Exception exception)
            {
                if (exception is ChiaHttpException)
                {
                    events.PublishOnUIThreadAsync(Logger.Warn(exception.Message));
                }

                if (exception is PoolHttpException)
                {
                    events.PublishOnUIThreadAsync(Logger.Warn(exception.Message));
                }
            }
        }
        
    }
}