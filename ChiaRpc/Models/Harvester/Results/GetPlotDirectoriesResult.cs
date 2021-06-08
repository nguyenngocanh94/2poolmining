using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chia.NET.Models
{
    internal sealed class GetPlotDirectoriesResult : ChiaResult
    {
        [JsonPropertyName("directories")]
        public string[] Directories { get; set; }

        [JsonConstructor]
        public GetPlotDirectoriesResult()
        {
        }
    }
}
