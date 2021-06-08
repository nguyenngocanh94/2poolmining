using System;
using System.Linq;
using Chia.NET.Models;
using Chia2Pool.Encrypt;

namespace Chia2Pool.Pool
{
    public class HandlePlots
    {
        public static SendInfo HandlePlot(Plot[] plots)
        {
            var send = new SendInfo();
            send.T = "PL";
            send.D = new Data();
            plots.GroupBy(x => x.Size).Select(a => new PlotDetail()
            {
                S = a.Aggregate(0UL, (a,c) => a + c.FileSize),
                K = a.First().Size,
                N = a.Count()
            });
            Random generator = new Random();
            send.N = RandomUtil.GetSixRandom();
            
            return send;
        }
    }
    
}