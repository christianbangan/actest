using GeneratorAPI.Models.Request;

namespace GeneratorAPI.Repositories.Interfaces
{
    public interface IOpenAIRepository
    {
        Task<IResult> GenerateYoutubeTitle(GenerateYoutubeTitleRequestModel body);

        Task<IResult> YoutubeChannelFinder(YoutubeChannelFinderRequestModel body);

        Task<IResult> YoutubePopularVideos(YoutubePopularVideosRequestModel body);
    }
}