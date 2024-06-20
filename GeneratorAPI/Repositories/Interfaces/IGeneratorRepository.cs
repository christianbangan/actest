using GeneratorAPI.Models.Request;

namespace GeneratorAPI.Repositories.Interfaces
{
    public interface IGeneratorRepository
    {
        Task<IResult> GenerateYoutubeTitle(GenerateYoutubeTitleRequestModel body);

        Task<IResult> YoutubeChannelFinder(YoutubeChannelFinderRequestModel body);

        Task<IResult> YoutubePopularVideos(YoutubePopularVideosRequestModel body);

        Task<IResult> HookGenerator(HookGeneratorRequestModel body);

        Task<IResult> KeywordSearchTool(KeywordSearchToolRequestModel body);
    }
}