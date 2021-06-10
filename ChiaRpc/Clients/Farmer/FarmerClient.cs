using Chia.NET.Clients.Farmer;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chia.NET.Models.Farmer;
using Chia.NET.Models.Farmer.Result;

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

        public async Task<GetRewardTargetResult> GetRewardTarget()
        {
            var data =  await PostAsync<GetRewardTargetResult>(FarmerRoutes.GetRewardTarget(ApiUrl),
                new Dictionary<string, bool>()
                {
                    ["search_for_private_key"] = false
                });
            return data;
        }

        public async Task SetRewardTargets(string targetAddress)
            => await PostAsync(FarmerRoutes.SetRewardTargets(ApiUrl), new Dictionary<string, string>()
            {
                ["farmer_target"] = targetAddress,
                ["pool_target"] = targetAddress,
            });
    }
}
