using Chia.NET.Parser;
using System.Net;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    public class ChiaConnection
    {
        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }

        [JsonPropertyName("bytes_read")]
        public int BytesRead { get; set; }

        [JsonPropertyName("bytes_written")]
        public int BytesWritten { get; set; }

        [JsonPropertyName("peer_host")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonIPAddressConverter))]
        public IPAddress PeerHost { get; set; }

        [JsonPropertyName("peer_port")]
        public ushort PeerPort { get; set; }

        [JsonPropertyName("peer_server_port")]
        public ushort PeerServerPort { get; set; }

        [JsonPropertyName("local_port")]
        public ushort LocalPort { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonConstructor]
        public ChiaConnection()
        {
        }
    }
}
