using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models.Farmer
{
    internal sealed class RewardTarget
    {
        [JsonPropertyName("farmer_target")] public string FarmerTarget { get; set; }

        [JsonPropertyName("pool_target")] public string PoolTarget { get; set; }

        [JsonPropertyName("have_farmer_sk")] public bool HaveFarmerSk { get; set; }

        [JsonPropertyName("have_pool_sk")] public string HavePoolSk { get; set; }

        [JsonConstructor]
        public RewardTarget()
        {
        }
    }
}