using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models.Farmer.Result
{
    public sealed class GetRewardTargetResult: ChiaResult
    {
        [JsonPropertyName("farmer_target")]
        public string FarmerTarget { get; set; }
        
        [JsonPropertyName("pool_target")]
        public string PoolTarget { get; set; }

        [JsonConstructor]
        public GetRewardTargetResult()
        {
        }
    }
}