namespace GeneratorAPI.Models.Request
{
    public class GenerateYoutubeTitleRequestModel
    {
        // parameters for Youtube Title Generator
        public string? ContentType { get; set; }

        public string? Keywords { get; set; }
    }
}