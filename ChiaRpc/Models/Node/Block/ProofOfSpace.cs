using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    public sealed class ProofOfSpace
    {
        [JsonPropertyName("challenge")]
        public string Challenge { get; set; }
        [JsonPropertyName("plot_public_key")]
        public string PlotPublicKey { get; set; }
        [JsonPropertyName("pool_public_key")]
        public string PoolPublicKey { get; set; }
        [JsonPropertyName("proof")]
        public string Proof { get; set; }
        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonConstructor]
        public ProofOfSpace()
        {
        }
    }
}
