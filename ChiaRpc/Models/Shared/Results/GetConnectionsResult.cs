using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    internal sealed class GetConnectionsResult : ChiaResult
    {
        [JsonPropertyName("connections")]
        public ChiaConnection[] Connections { get; set; }

        [JsonConstructor]
        public GetConnectionsResult()
        {
        }
    }
}
