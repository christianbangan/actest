namespace GeneratorAPI.Models.Response
{
    public class BaseResponseModel
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Result { get; set; }
    }
}