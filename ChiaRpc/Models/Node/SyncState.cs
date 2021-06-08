using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    public sealed class SyncState
    {
        [JsonPropertyName("sync_mode")]
        public bool Mode { get; set; }

        [JsonPropertyName("sync_progress_height")]
        public long ProgressHeight { get; set; }

        [JsonPropertyName("sync_tip_height")]
        public long TipHeight { get; set; }

        [JsonPropertyName("synced")]
        public bool Synced { get; set; }

        [JsonConstructor]
        public SyncState()
        {
        }
    }
}
