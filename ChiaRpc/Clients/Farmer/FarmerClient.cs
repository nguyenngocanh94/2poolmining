using Chia.NET.Clients.Farmer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chia.NET.Clients
{
    public sealed class FarmerClient : ChiaApiClient
    {
        private const string ApiUrl = "https://localhost:8559/";

        public FarmerClient(string ssl)
            : base(ssl,"farmer", ApiUrl)
        {
            InitializeAsync();
        }

        public async Task SetRewardTargets(string targetAddress)
            => await PostAsync(FarmerRoutes.SetRewardTargets(ApiUrl), new Dictionary<string, string>()
            {
                ["farmer_target"] = targetAddress,
                ["pool_target"] = targetAddress,
            });
    }
}
