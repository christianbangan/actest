namespace GeneratorAPI.Models.Response
{
    public class OpenAIErrorDetailsResponse
    {
        public string? Message { get; set; }
        public string? Type { get; set; }
        public object? Param { get; set; }
        public string? Code { get; set; }
    }

    public class OpenAIErrorResponse
    {
        public OpenAIErrorDetailsResponse? Error { get; set; }
    }
}