using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    internal sealed class GetPlotsResult : ChiaResult
    {
        [JsonPropertyName("failed_to_open_filenames")]
        public string[] FailedToOpenFileNames { get; set; }

        [JsonPropertyName("not_found_filenames")]
        public string[] NotFoundFilenames { get; set; }

        [JsonPropertyName("plots")]
        public Plot[] Plots { get; set; }

        [JsonConstructor]
        public GetPlotsResult()
        {
        }
    }
}
