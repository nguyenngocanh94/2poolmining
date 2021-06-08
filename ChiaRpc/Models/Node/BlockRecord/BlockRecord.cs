using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    public sealed class BlockRecord
    {
        [JsonPropertyName("header_hash")]
        public string HeaderHash { get; set; }
        [JsonPropertyName("height")]
        public long Height { get; set; }
        [JsonPropertyName("weight")]
        public long Weight { get; set; }

        [JsonConstructor]
        public BlockRecord()
        {

        }
    }
}
