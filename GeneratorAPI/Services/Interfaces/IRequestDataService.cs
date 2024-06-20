using GeneratorAPI.Models.Request;
using GeneratorAPI.Models.Response;

namespace GeneratorAPI.Services.Interfaces
{
    public interface IRequestDataService
    {
        Task<object> GenerateYoutubeTitle(GenerateYoutubeTitleRequestModel body);

        Task<YoutubeResponseModel> YoutubeChannelFinder(YoutubeChannelFinderRequestModel body);

        Task<YoutubeResponseModel> YoutubePopularVideos(YoutubePopularVideosRequestModel body);

        Task<object> HookGenerator(HookGeneratorRequestModel body);

        Task<object> KeywordSearchTool(KeywordSearchToolRequestModel body);
    }
}