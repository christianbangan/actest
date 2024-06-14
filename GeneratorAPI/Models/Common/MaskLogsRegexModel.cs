using Newtonsoft.Json;

namespace GeneratorAPI.Models.Common
{
    public class MaskLogsRegexModel
    {
        [JsonProperty("pattern")]
        public string? Pattern { get; set; }

        [JsonProperty("replacement")]
        public string? Replacement { get; set; }
    }
}