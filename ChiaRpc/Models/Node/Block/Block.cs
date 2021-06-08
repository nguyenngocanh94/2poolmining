using System.Text.Json.Serialization;

namespace Chia.NET.Models
{
    public sealed class Block
    {
        [JsonPropertyName("header_hash")]
        public string HeaderHash { get; set; }
    }
}
