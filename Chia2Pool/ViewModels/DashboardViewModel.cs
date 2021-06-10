using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Chia2Pool.Chia;
using Chia2Pool.Common;
using Chia2Pool.Messages;
using Chia2Pool.Models;
using Chia2Pool.Periodical;
using Chia2Pool.Pool;
using Action = System.Action;

namespace Chia2Pool.ViewModels
{
    [Export(typeof(DashboardViewModel))]
    public class DashboardViewModel : BaseViewModel, IHandle<LogMessage>
    {
        public static string star = "***********************";
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;

        private SendInfoPeriodical sendInfoPeriodical;
        private SetRewardPeriodical setRewardPeriodical;
        private readonly Settings _settings;

        public bool IsEnableButton { get; set; }
        public bool IsReadOnly { get; set; }

        [ImportingConstructor]
        public DashboardViewModel(IEventAggregator events, IWindowManager windowManager) : base(events)
        {
            _windowManager = windowManager;
            _eventAggregator = events;
            LogEntries = new ObservableCollection<LogEntry>();
            _settings = Settings.GetInstance();
            _settings.Fetch();
            if (Settings.GetInstance().ApiKey != null)
            {
                ApiKey = star;
                IsReadOnly = true;
            }

            if (Settings.GetInstance().SslDirectory != null)
            {
                SslDirectory = star;
            }

            if (_settings.ApiKey != null && _settings.SslDirectory != null)
            {
                sendInfoPeriodical = new SendInfoPeriodical(events);
                setRewardPeriodical = new SetRewardPeriodical(events);
            }
            IsEnableButton = true;
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
            set
            {
                _logEntries = value;
                NotifyOfPropertyChange(() => _logEntries);
            }
        }

        public ICommand StartMiningCommand { get; protected set; }

        protected async void ExecuteStartMining()
        {
            if (ApiKey != null && SslDirectory != null)
            {
                try
                {
                    var apikey = "";
                    var ssl = "";
                    if (ApiKey == star && SslDirectory == star)
                    {
                        apikey = _settings.ApiKey;
                        ssl = _settings.SslDirectory;
                    }
                    else
                    {
                        apikey = ApiKey;
                        ssl = SslDirectory;
                        _settings.ApiKey = apikey;
                        _settings.SslDirectory = ssl;
                    }

                    PoolClient pc = new PoolClient(Settings.GetInstance().PoolUrl, apikey);
                    ChiaController chia = new ChiaController(ssl);
                    // backup
                    var rewardTarget = await chia.FarmerClient.GetRewardTarget();
                    _settings.PoolTarget = rewardTarget.PoolTarget;
                    _settings.FarmerTarget = rewardTarget.FarmerTarget;
                    await _settings.Persist();

                    var data = await chia.FullNodeClient.GetBlockchainStateAsync();
                    if (!data.SyncState.Synced)
                    {
                        AddLog(LogLevel.WARN, "block is not synced, can not mine ...");
                        return;
                    }

                    // handle
                    var poolInfo = pc.GetPoolInfo();
                    await chia.FarmerClient.SetRewardTargets(poolInfo.data.pa);
                    var plots = await chia.HarvesterClient.GetPlotsAsync();
                    pc.SendData(HandlePlots.HandlePlot(plots));
                    var dueTime = TimeSpan.FromSeconds(5);
                    var interval = TimeSpan.FromSeconds(15);
                    _ = RunPeriodicAsync(SetReward, dueTime, interval, CancellationToken.None);
                    var dueTime1 = TimeSpan.FromSeconds(10);
                    var interval2 = TimeSpan.FromSeconds(25);
                    _ = RunPeriodicAsync(SendPlot, dueTime1, interval2, CancellationToken.None);
                    IsEnableButton = false;
                    NotifyOfPropertyChange(() => IsEnableButton);
                }
                catch (Exception e)
                {
                    LogEntries.Add(Logger.W(e.Message));
                }
            }
        }

        private void AddLog(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.WARN:
                    LogEntries.Add(Logger.W(message));
                    break;
                case LogLevel.INFO:
                    LogEntries.Add(Logger.I(message));
                    break;
                case LogLevel.FATAL:
                    LogEntries.Add(Logger.F(message));
                    break;
            }
        }

        public void Handle(LogMessage message)
        {
            LogEntries.Add(new LogEntry()
            {
                Message = message.Message,
                DateTime = message.Date,
                Level = message.LogLevel.ToString()
            });
        }


        private static async Task RunPeriodicAsync(Action onTick,
            TimeSpan dueTime,
            TimeSpan interval,
            CancellationToken token)
        {
            // Initial wait time before we begin the periodic loop.
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token);

            // Repeat this loop until cancelled.
            while (!token.IsCancellationRequested)
            {
                // Call our onTick function.
                onTick?.Invoke();

                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);
            }
        }


        private void SendPlot()
        {
            if (sendInfoPeriodical == null)
            {
                sendInfoPeriodical = new SendInfoPeriodical(_eventAggregator);
            }

            sendInfoPeriodical.Run();
        }

        private void SetReward()
        {
            if (setRewardPeriodical == null)
            {
                setRewardPeriodical = new SetRewardPeriodical(_eventAggregator);
            }

            setRewardPeriodical.Run();
        }
    }
}