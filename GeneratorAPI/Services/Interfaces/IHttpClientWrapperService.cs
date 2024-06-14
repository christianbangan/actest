using GeneratorAPI.Models.Common;

namespace GeneratorAPI.Services.Interfaces
{
    public interface IHttpClientWrapperService
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, List<HeadersModel>? headers = null);
    }
}