using Newtonsoft.Json;

namespace GeneratorAPI.Models.Request
{
    public class Message
    {
        [JsonProperty("role")]
        public string? Role { get; set; }

        [JsonProperty("content")]
        public string? Content { get; set; }
    }

    public class GenerateYoutubeTitleRequestModel
    {
        [JsonProperty("model")]
        public string? Model { get; set; }

        [JsonProperty("messages")]
        public List<Message>? Messages { get; set; }

        [JsonProperty("max_tokens")]
        public int MaxTokens { get; set; }
    }
}