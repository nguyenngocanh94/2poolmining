using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    public sealed class BlockData
    {
        [JsonPropertyName("farmer_reward_puzzle_hash")]
        public string FarmerRewardPuzzleHash { get; set; }
        [JsonPropertyName("pool_signature")]
        public string PoolSignature { get; set; }
        [JsonPropertyName("pool_target")]
        public PoolTarget PoolTarget { get; set; }

        [JsonConstructor]
        public BlockData()
        {
        }
    }
}
