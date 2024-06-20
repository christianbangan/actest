using GeneratorAPI.Models.Request;
using GeneratorAPI.Repositories.Interfaces;

namespace GeneratorAPI.ExtensionMethods
{
    public static class Endpoints
    {
        private static async Task<IResult> HandleRequest(object body, IGeneratorRepository _repository, string methodName)
        {
            return methodName switch
            {
                "GenerateYoutubeTitle" => await _repository.GenerateYoutubeTitle((GenerateYoutubeTitleRequestModel)body),
                "YoutubeChannelFinder" => await _repository.YoutubeChannelFinder((YoutubeChannelFinderRequestModel)body),
                "YoutubePopularVideos" => await _repository.YoutubePopularVideos((YoutubePopularVideosRequestModel)body),
                "HookGenerator" => await _repository.HookGenerator((HookGeneratorRequestModel)body),
                "KeywordSearchTool" => await _repository.KeywordSearchTool((KeywordSearchToolRequestModel)body),
                "VideoDescriptionGenerator" => await _repository.VideoDescriptionGenerator((VideoDescriptionRequestModel)body),
                "VideoDescriptionEmailGenerator" => await _repository.VideoDescriptionEmailGenerator((VideoDescriptionEmailRequestModel)body),
                _ => throw new NotImplementedException("Method not implemented."),
            };
        }

        public static void MapEndpoints(this WebApplication app)
        {
            // Endpoint for generating youtube title
            app.MapPost("/api/GenerateYoutubeTitle", async (GenerateYoutubeTitleRequestModel body, IGeneratorRepository _repository) =>
            {
                return await HandleRequest(body, _repository, "GenerateYoutubeTitle");
            });

            // Endpoint for finding youtube channels
            app.MapPost("/api/YoutubeChannelFinder", async (YoutubeChannelFinderRequestModel body, IGeneratorRepository _repository) =>
            {
                return await HandleRequest(body, _repository, "YoutubeChannelFinder");
            });

            // Endpoint for finding youtube videos
            app.MapPost("/api/YoutubePopularVideos", async (YoutubePopularVideosRequestModel body, IGeneratorRepository _repository) =>
            {
                return await HandleRequest(body, _repository, "YoutubePopularVideos");
            });

            // Endpoint for hook generator
            app.MapPost("/api/HookGenerator", async (HookGeneratorRequestModel body, IGeneratorRepository _repository) =>
            {
                return await HandleRequest(body, _repository, "HookGenerator");
            });

            // Endpoint for keyword search tool
            app.MapPost("/api/KeywordSearchTool", async (KeywordSearchToolRequestModel body, IGeneratorRepository _repository) =>
            {
                return await HandleRequest(body, _repository, "KeywordSearchTool");
            });

            app.MapPost("/api/VideoDescriptionGenerator", async (VideoDescriptionRequestModel body, IGeneratorRepository _repository) =>
            {
                return await HandleRequest(body, _repository, "VideoDescriptionGenerator");
            });

            app.MapPost("/api/VideoDescriptionEmailGenerator", async (VideoDescriptionEmailRequestModel body, IGeneratorRepository _repository) =>
            {
                return await HandleRequest(body, _repository, "VideoDescriptionEmailGenerator");
            });
        }
    }
}