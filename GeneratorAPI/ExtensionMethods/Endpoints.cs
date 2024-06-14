using GeneratorAPI.Models.Request;
using GeneratorAPI.Repositories;

namespace GeneratorAPI.ExtensionMethods
{
    public static class Endpoints
    {
        private static async Task<IResult> HandleRequest(object body, IOpenAIRepository _repository, string methodName)
        {
            return methodName switch
            {
                "GenerateYoutubeTitle" => await _repository.GenerateYoutubeTitle((RequestModel)body),
                _ => throw new NotImplementedException("Method not implemented."),
            };
        }

        public static void MapEndpoints(this WebApplication app)
        {
            // Endpoint for Generating Youtube Title
            app.MapPost("/api/GenerateYoutubeTitle", async (RequestModel body, IOpenAIRepository _repository) =>
            {
                return await HandleRequest(body, _repository, "GenerateYoutubeTitle");
            });
        }
    }
}