using GeneratorAPI.Models.Request;
using System.Net;

namespace GeneratorAPI.Services.Interfaces
{
    public interface IRequestDataService
    {
        Task<object> GenerateYoutubeTitle(RequestModel body);
    }
}