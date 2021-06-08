using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    internal sealed class GetBlocksResult : ChiaResult
    {
        [JsonPropertyName("blocks")]
        public Block[] Blocks { get; set; }

        [JsonConstructor]
        public GetBlocksResult()
        {
        }
    }
}
