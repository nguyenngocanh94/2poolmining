using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    public class ChiaResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonConstructor]
        public ChiaResult()
        {
        }
    }
}
