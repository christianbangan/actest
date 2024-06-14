using GeneratorAPI.Models.Request;

namespace GeneratorAPI.Repositories
{
    public interface IOpenAIRepository
    {
        Task<IResult> GenerateYoutubeTitle(RequestModel body);
    }
}