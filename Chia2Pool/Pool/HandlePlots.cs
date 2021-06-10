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
            send.t = "PL";
            send.d = new Data();
            plots.GroupBy(x => x.Size).Select(a => new PlotDetail()
            {
                s = a.Aggregate(0UL, (a,c) => a + c.FileSize),
                k = a.First().Size,
                n = a.Count()
            });
            Random generator = new Random();
            send.n = RandomUtil.GetSixRandom();
            
            return send;
        }
    }
    
}