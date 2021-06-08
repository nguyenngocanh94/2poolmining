using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    public sealed class Foliage
    {
        [JsonPropertyName("foliage_block_data_signature")]
        public string BlockDataSignature { get; set; }
        [JsonPropertyName("foliage_block_data")]
        public BlockData BlockData { get; set; }
        [JsonPropertyName("reward_block_hash")]
        public string RewardBlockHash { get; set; }

        [JsonConstructor]
        public Foliage()
        {
        }
    }
}
