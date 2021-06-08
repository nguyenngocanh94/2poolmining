using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    internal sealed class GetBlockchainStateResult : ChiaResult
    {
        [JsonPropertyName("blockchain_state")]
        public BlockchainState BlockchainState { get; set; }

        [JsonConstructor]
        public GetBlockchainStateResult()
        {
        }
    }
}
