using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    public sealed class Plot
    {
        [JsonPropertyName("file_size")]
        public ulong FileSize { get; set; }

        [JsonPropertyName("plot-seed")]
        public string Seed { get; set; }

        [JsonPropertyName("filename")]
        public string FileName { get; set; }

        [JsonPropertyName("plot_public_key")]
        public string PublicKey { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonConstructor]
        public Plot()
        {
        }
    }
}
