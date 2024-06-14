using GeneratorAPI.Models.Common;
using GeneratorAPI.Services.Interfaces;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace GeneratorAPI.Services
{
    public class HttpClientWrapperService : IHttpClientWrapperService
    {
        private readonly HttpClient _httpClient;

        public HttpClientWrapperService(HttpClient httpClient)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += ValidateCertificate!;

            httpClient = new HttpClient(handler);

            _httpClient = httpClient;
        }

        // for bypassing SSL
        private static bool ValidateCertificate(HttpRequestMessage request, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, List<HeadersModel>? headers = null)
        {
            if (headers != null && headers.Count > 0)
            {
                var headersList = headers.Where(header =>
                    !string.IsNullOrEmpty(header.HeaderName) && !string.IsNullOrEmpty(header.HeaderValue));

                foreach (var header in headersList)
                    _httpClient.DefaultRequestHeaders.Add(header.HeaderName!, header.HeaderValue);
            }

            var result = await _httpClient.SendAsync(request);

            return result;
        }
    }
}