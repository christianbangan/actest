using Newtonsoft.Json;

namespace GeneratorAPI.Models.Response
{
    public class HookGeneratorApiResponseModel
    {
        [JsonProperty("Intriguing_Question")]
        public string? IntriguingQuestion { get; set; }

        [JsonProperty("Visual_Imagery")]
        public string? VisualImagery { get; set; }

        [JsonProperty(nameof(Quotation))]
        public string? Quotation { get; set; }
    }
}