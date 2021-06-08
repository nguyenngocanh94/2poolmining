using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    public sealed class Wallet
    {
        [JsonPropertyName("wallet_id")]
        public int Id { get; set; }

        [JsonPropertyName("confirmed_wallet_balance")]
        public double ConfirmedBalance { get; set; }

        [JsonPropertyName("unconfirmed_wallet_balance")]
        public double UnconfirmedBalance { get; set; }

        [JsonPropertyName("spendable_balance")]
        public double SpendableBalance { get; set; }

        [JsonPropertyName("max_send_amount")]
        public double MaxSendAmount { get; set; }

        [JsonPropertyName("pending_change")]
        public double PendingChange { get; set; }

        public double Percentage { get; set; }

        [JsonConstructor]
        public Wallet()
        {
            Percentage = 100;
        }

        public static Wallet Empty => new Wallet() { Percentage = 0 };
    }
}
