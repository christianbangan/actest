using GeneratorAPI.Models.Request;
using GeneratorAPI.Repositories.Interfaces;

namespace GeneratorAPI.ExtensionMethods
{
    public static class Endpoints
    {
        private static async Task<IResult> HandleRequest(object body, IOpenAIRepository _repository, string methodName)
        {
            return methodName switch
            {
                "GenerateYoutubeTitle" => await _repository.GenerateYoutubeTitle((GenerateYoutubeTitleRequestModel)body),
                "YoutubeChannelFinder" => await _repository.YoutubeChannelFinder((YoutubeChannelFinderRequestModel)body),
                //"YoutubePopularVideos" => await _repository.YoutubePopularVideos((YoutubePopularVideosRequestModel)body),
                "HookGenerator" => await _repository.HookGenerator((HookGeneratorRequestModel)body),
                _ => throw new NotImplementedException("Method not implemented."),
            };
        }

        public static void MapEndpoints(this WebApplication app)
        {
            // Endpoint for generating youtube title
            app.MapPost("/api/GenerateYoutubeTitle", async (GenerateYoutubeTitleRequestModel body, IOpenAIRepository _repository) =>
            {
                return await HandleRequest(body, _repository, "GenerateYoutubeTitle");
            });

            // Endpoint for finding youtube channels
            app.MapPost("/api/YoutubeChannelFinder", async (YoutubeChannelFinderRequestModel body, IOpenAIRepository _repository) =>
            {
                return await HandleRequest(body, _repository, "YoutubeChannelFinder");
            });

            // Endpoint for finding youtube videos
            app.MapPost("/api/YoutubePopularVideos", async (YoutubePopularVideosRequestModel body, IOpenAIRepository _repository) =>
            {
                return await HandleRequest(body, _repository, "YoutubePopularVideos");
            });

            // Endpoimt fpr hook generator
            app.MapPost("/api/HookGenerator", async (HookGeneratorRequestModel body, IOpenAIRepository _repository) =>
            {
                return await HandleRequest(body, _repository, "HookGenerator");
            });
        }
    }
}