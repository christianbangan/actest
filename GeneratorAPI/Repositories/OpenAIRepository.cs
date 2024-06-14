using GeneratorAPI.Models.Request;
using GeneratorAPI.Services.Interfaces;

namespace GeneratorAPI.Repositories
{
    public class OpenAIRepository(IRequestDataService requestData) : IOpenAIRepository
    {
        private readonly IRequestDataService _requestData = requestData;

        public async Task<IResult> GenerateYoutubeTitle(RequestModel body)
        {
            var response = await _requestData.GenerateYoutubeTitle(body);

            if (response is string)
                return Results.BadRequest(response);
            else
                return Results.Ok(response);
        }
    }
}