using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Chia.NET.Clients;
using Chia.NET.Models;
using WindowUtil = Chia2Pool.Windows.Window;

namespace Chia2Pool.Chia
{
    public class ChiaController
    {
        public FullNodeClient FullNodeClient;
        public WalletClient WalletClient;
        public HarvesterClient HarvesterClient;
        public FarmerClient FarmerClient;
        
        public ChiaController(string sslDirectory)
        {
            FullNodeClient = new FullNodeClient(sslDirectory);
            WalletClient = new WalletClient(sslDirectory);
            HarvesterClient = new HarvesterClient(sslDirectory);
            FarmerClient = new FarmerClient(sslDirectory);
        }
        
        public async Task<List<ChiaResult>> StopAll()
        {
            List<ChiaResult> cr = new List<ChiaResult>();
            var f = await FullNodeClient.StopNode();
            cr.Add(f);
            var h = await HarvesterClient.StopNode();
            cr.Add(h);
            var fa = await FarmerClient.StopNode();
            cr.Add(fa);
            var w = await WalletClient.StopNode();
            cr.Add(w);
            return cr;
        }

        public void StopByPort()
        {
            int[] ports = new[] {55400, 8555, 8559, 8560, 9256};
            foreach (var port in ports)
            {
                WindowUtil.KillProcessOnPort(port);
            }
        }

        public bool EnsureSyncedStarted()
        {
            while (true)
            {
                var state  = FullNodeClient.GetBlockchainState();
                if (state.SyncState.Synced)
                {
                    return true;
                }

                // 2s call 1 lan
                Thread.Sleep(1000 * 2);
            }
        }

        public void RunChiaExe()
        {
            Process.Start(@"C:\Users\"+Environment.UserName+@"\AppData\Local\chia-blockchain\app-1.1.6\resources\app.asar.unpacked\daemon\Chia.exe", " start farmer");
        }
        
        
    }
}