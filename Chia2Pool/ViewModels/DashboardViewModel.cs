using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using Chia2Pool.Chia;
using Chia2Pool.Common;
using Chia2Pool.Messages;
using Chia2Pool.Models;
using Chia2Pool.Periodical;
using Chia2Pool.Pool;

namespace Chia2Pool.ViewModels
{
    [Export(typeof(DashboardViewModel))]
    public class DashboardViewModel : BaseViewModel, IHandle<LogMessage>
    {
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        [ImportingConstructor]
        public DashboardViewModel(IEventAggregator events, IWindowManager windowManager) : base(events)
        {
            _windowManager = windowManager;
            _eventAggregator = events;
            LogEntries = new ObservableCollection<LogEntry>();
            if (Settings.GetInstance().ApiKey!=null)
            {
                ApiKey = Settings.GetInstance().ApiKey;
            }

            if (Settings.GetInstance().SslDirectory!=null)
            {
                SslDirectory = Settings.GetInstance().SslDirectory;
            }
        }
        
        private string _apiKey;
        public string ApiKey
        {
            get => _apiKey;
            set => _apiKey = value;
        }
        private string _sslDirectory;

        public string SslDirectory
        {
            get => _sslDirectory;
            set => _sslDirectory = value;
        }

        private ObservableCollection<LogEntry> _logEntries;
        public ObservableCollection<LogEntry> LogEntries
        {
            get => _logEntries;
            set { _logEntries = value; NotifyOfPropertyChange(() => _logEntries); }
        }

        public ICommand StartMiningCommand { get; protected set; }
        
        protected async void ExecuteStartMining()
        {
            if (ApiKey != null && SslDirectory != null)
            {
                try
                {
                    PoolClient pc = new PoolClient(Settings.GetInstance().PoolUrl, ApiKey);
                    ChiaController chia = new ChiaController(SslDirectory);
                    var poolInfo = pc.GetPoolInfo();
                    await chia.FarmerClient.SetRewardTargets(poolInfo.Data.Pa);
                    var plots = await chia.HarvesterClient.GetPlotsAsync();
                    pc.SendData(HandlePlots.HandlePlot(plots));

                    SendInfoPeriodical sendInfoPeriodical = new SendInfoPeriodical(5, _eventAggregator);
                    SetRewardPeriodical setRewardPeriodical = new SetRewardPeriodical(5,poolInfo.Data.Pa,_eventAggregator);
                }
                catch (Exception e)
                {
                    LogEntries.Add(Logger.W(e.Message));
                }
            }
        }

        public void Handle(LogMessage message)
        {
            LogEntries.Add(new LogEntry()
            {
                Message = message.Message,
                DateTime = message.Date
            });
        }
    }
}