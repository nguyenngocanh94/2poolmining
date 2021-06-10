using System;
using System.Threading.Tasks;
using Chia.NET.Clients;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var c = new FarmerClient(@"C:\Users\Admin\.chia\mainnet\config\ssl");
            var f = new WalletClient(@"C:\Users\Admin\.chia\mainnet\config\ssl");
            await c.GetRewardTarget();
        }
    }
}