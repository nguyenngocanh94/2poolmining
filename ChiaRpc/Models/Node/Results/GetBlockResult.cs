using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    internal sealed class GetBlockResult : ChiaResult
    {
        [JsonPropertyName("block")]
        public Block Block { get; set; }

        [JsonConstructor]
        public GetBlockResult()
        {
        }
    }
}
