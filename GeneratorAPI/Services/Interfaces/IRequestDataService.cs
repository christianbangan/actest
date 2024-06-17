using GeneratorAPI.Models.Request;
using System.Net;

namespace GeneratorAPI.Services.Interfaces
{
    public interface IRequestDataService
    {
        Task<object> GenerateYoutubeTitle(GenerateYoutubeTitleRequestModel body);

        Task<object> YoutubeChannelFinder(YoutubeChannelFinderRequestModel body);

        Task<object> YoutubePopularVideos(YoutubePopularVideosRequestModel body);
    }
}