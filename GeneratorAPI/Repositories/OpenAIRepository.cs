using FluentValidation;
using GeneratorAPI.Models.Request;
using GeneratorAPI.Models.Response;
using GeneratorAPI.Repositories.Interfaces;
using GeneratorAPI.Services.Interfaces;
using System.Net;

namespace GeneratorAPI.Repositories
{
    public class OpenAIRepository(IRequestDataService requestData, IValidator<GenerateYoutubeTitleRequestModel> generateYoutubeTitleValidator, IValidator<YoutubeChannelFinderRequestModel> youtubeChannelFinderRequestValidadtor, ILoggerService logger) : IOpenAIRepository
    {
        private readonly IRequestDataService _requestData = requestData;
        private readonly IValidator<GenerateYoutubeTitleRequestModel> _generateYoutubeTitleValidator = generateYoutubeTitleValidator;
        private readonly IValidator<YoutubeChannelFinderRequestModel> _youtubeChannelFinderRequestValidadtor = youtubeChannelFinderRequestValidadtor;
        private readonly ILoggerService _logger = logger;

        private async Task<string> ValidateRequestParameters(object body)
        {
            string errorMessage = string.Empty;

            FluentValidation.Results.ValidationResult validate = new();

            if (body is GenerateYoutubeTitleRequestModel title)
                validate = await _generateYoutubeTitleValidator.ValidateAsync(title);
            else if (body is YoutubeChannelFinderRequestModel channel)
                validate = await _youtubeChannelFinderRequestValidadtor.ValidateAsync(channel);

            if (!validate.IsValid)
            {
                errorMessage = validate.Errors[0].ErrorMessage;
                await _logger.Log($"Error encountered (parameter error): {errorMessage}");
            }

            return errorMessage;
        }

        private async Task<IResult> CallOpenAIAPI(object body)
        {
            var response = new GenerateYoutubeTitleResponseModel { StatusCode = (int)HttpStatusCode.BadRequest };

            try
            {
                var reqParam = await ValidateRequestParameters(body);

                if (!string.IsNullOrEmpty(reqParam))
                {
                    response.Message = reqParam;
                    return Results.BadRequest(response);
                }
                else
                {
                    object? result = null;

                    if (body is GenerateYoutubeTitleRequestModel generateYtTitle)
                        result = await _requestData.GenerateYoutubeTitle(generateYtTitle);
                    else if (body is HookGeneratorRequestModel hookApi)
                        result = await _requestData.HookGenerator(hookApi);

                    if (result is string res)
                    {
                        response.Message = res;
                        return Results.BadRequest(response);
                    }
                    else
                    {
                        response.StatusCode = (int)HttpStatusCode.OK;
                        response.Message = result;
                        return Results.Ok(response);
                    }
                }
            }
            catch (Exception e)
            {
                await _logger.Log($"Error encountered: {e.Message}");
                response.Message = e.Message;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Results.Json(response, options: null, contentType: null, statusCode: 500);
            }
        }

        public async Task<IResult> GenerateYoutubeTitle(GenerateYoutubeTitleRequestModel body)
        {
            try
            {
                var response = await CallOpenAIAPI(body);
                return response;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IResult> YoutubeChannelFinder(YoutubeChannelFinderRequestModel body)
        {
            try
            {
                var reqParam = await ValidateRequestParameters(body);

                if (!string.IsNullOrEmpty(reqParam))
                {
                    var result = new YoutubeChannelFinderFailedResponseModel
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Error = reqParam
                    };

                    return Results.BadRequest(result);
                }

                var response = await _requestData.YoutubeChannelFinder(body);

                if (response is YoutubeChannelFinderFailedResponseModel)
                    return Results.BadRequest(response);

                return Results.Ok(response);
            }
            catch (Exception e)
            {
                YoutubeChannelFinderFailedResponseModel response = new()
                {
                    StatusCode = 500,
                    Error = e.Message
                };

                await _logger.Log($"Error encountered: {e.Message}");

                return Results.Json(response, options: null, contentType: null, statusCode: 500);
            }
        }

        public async Task<IResult> YoutubePopularVideos(YoutubePopularVideosRequestModel body)
        {
            try
            {
                var reqParam = await ValidateRequestParameters(body);

                if (!string.IsNullOrEmpty(reqParam))
                {
                    var result = new YoutubeChannelFinderFailedResponseModel
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Error = reqParam
                    };

                    return Results.BadRequest(result);
                }

                var response = await _requestData.YoutubePopularVideos(body);

                if (response is YoutubeChannelFinderFailedResponseModel)
                    return Results.BadRequest(response);

                return Results.Ok(response);
            }
            catch (Exception e)
            {
                YoutubeChannelFinderFailedResponseModel response = new()
                {
                    StatusCode = 500,
                    Error = e.Message
                };

                await _logger.Log($"Error encountered: {e.Message}");

                return Results.Json(response, options: null, contentType: null, statusCode: 500);
            }
        }

        public async Task<IResult> HookGenerator(HookGeneratorRequestModel body)
        {
            try
            {
                var response = await CallOpenAIAPI(body);
                return response;
            }
            catch
            {
                throw;
            }
        }
    }
}