using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    public sealed class GetWalletBalanceResult : ChiaResult
    {
        [JsonPropertyName("wallet_balance")]
        public Wallet Wallet { get; set; }

        [JsonConstructor]
        public GetWalletBalanceResult()
        {
        }
    }
}
