using Chia.NET.Parser;
using System.Numerics;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    public sealed class BlockchainState
    {
        [JsonPropertyName("difficulty")]
        public int Difficulty { get; set; }

        [JsonPropertyName("genesis_challenge_initialized")]
        public bool GenesisChallengeInitiated { get; set; }

        [JsonPropertyName("peak")]
        public BlockRecord Peak { get; set; }

        [JsonPropertyName("space")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonBigIntegerConverter))]
        public BigInteger Space { get; set; }

        [JsonPropertyName("sync")]
        public SyncState SyncState { get; set; }

        [JsonConstructor]
        public BlockchainState()
        {
        }
    }
}
