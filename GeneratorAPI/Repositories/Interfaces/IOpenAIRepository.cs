using GeneratorAPI.Models.Request;

namespace GeneratorAPI.Repositories.Interfaces
{
    public interface IOpenAIRepository
    {
        Task<IResult> GenerateYoutubeTitle(RequestModel body);
    }
}