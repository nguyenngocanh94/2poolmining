using System.Text.Json.Serialization;

namespace Chia.NET.Models
{
    public sealed class GetWalletAddressResult : ChiaResult
    {
        [JsonPropertyName("wallet_id")]
        public int Id { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        public GetWalletAddressResult()
        {
        }
    }
}
