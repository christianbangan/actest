using Newtonsoft.Json;

namespace GeneratorAPI.Models.Response
{
    public class KeywordSearchToolApiResponseModel
    {
        [JsonProperty("average_search_volume")]
        public string? AverageSearchVolume { get; set; }

        [JsonProperty("average_search_meter")]
        public string? AverageSearchMeter { get; set; }

        [JsonProperty("average_rank_difficulty")]
        public string? AverageRankDifficulty { get; set; }

        [JsonProperty("attention_grabbing_tips")]
        public List<string>? AIAnalysis { get; set; }

        [JsonProperty("related_keywords")]
        public List<RelatedKeywords>? RelatedKeywords { get; set; }
    }

    public class RelatedKeywords
    {
        public string? Keyword { get; set; }

        [JsonProperty("search_volume")]
        public string? SearchVolume { get; set; }

        [JsonProperty("rank_difficulty")]
        public string? RankDifficulty { get; set; }
    }
}