using FluentValidation;
using GeneratorAPI.Models.Request;
using GeneratorAPI.Models.Response;
using GeneratorAPI.Repositories.Interfaces;
using GeneratorAPI.Services.Interfaces;

namespace GeneratorAPI.Repositories
{
    public class GeneratorRepository(IRequestDataService requestData,
        IValidator<GenerateYoutubeTitleRequestModel> generateYoutubeTitleValidator,
        IValidator<YoutubeChannelFinderRequestModel> youtubeChannelFinderRequestValidadtor,
        IValidator<HookGeneratorRequestModel> hookGeneratorValidator,
        IValidator<YoutubePopularVideosRequestModel> youtubePopularVideosValidator,
        IValidator<KeywordSearchToolRequestModel> keywordSearchToolValidator,
        IValidator<VideoDescriptionRequestModel> videoDescriptionModelValidator,
        IValidator<VideoDescriptionEmailRequestModel> videoDescriptionEmailValidator,
        ILoggerService logger) : IGeneratorRepository
    {
        private readonly IRequestDataService _requestData = requestData;
        private readonly IValidator<GenerateYoutubeTitleRequestModel> _generateYoutubeTitleValidator = generateYoutubeTitleValidator;
        private readonly IValidator<YoutubeChannelFinderRequestModel> _youtubeChannelFinderRequestValidator = youtubeChannelFinderRequestValidadtor;
        private readonly IValidator<HookGeneratorRequestModel> _hookGeneratorValidator = hookGeneratorValidator;
        private readonly IValidator<KeywordSearchToolRequestModel> _keywordSearchToolValidator = keywordSearchToolValidator;
        private readonly IValidator<YoutubePopularVideosRequestModel> _youtubePopularVideosValidator = youtubePopularVideosValidator;
        private readonly IValidator<VideoDescriptionRequestModel> _videoDescriptionValidator = videoDescriptionModelValidator;
        private readonly IValidator<VideoDescriptionEmailRequestModel> _videoDescriptionEmailValidator = videoDescriptionEmailValidator;
        private readonly ILoggerService _logger = logger;

        private async Task<string> ValidateRequestParameters(object body)
        {
            string errorMessage = string.Empty;

            FluentValidation.Results.ValidationResult validate = new();

            if (body is GenerateYoutubeTitleRequestModel title)
                validate = await _generateYoutubeTitleValidator.ValidateAsync(title);
            else if (body is YoutubeChannelFinderRequestModel channel)
                validate = await _youtubeChannelFinderRequestValidator.ValidateAsync(channel);
            else if (body is HookGeneratorRequestModel generator)
                validate = await _hookGeneratorValidator.ValidateAsync(generator);
            else if (body is KeywordSearchToolRequestModel keywordSearchTool)
                validate = await _keywordSearchToolValidator.ValidateAsync(keywordSearchTool);
            else if (body is YoutubePopularVideosRequestModel youtubePopularVideos)
                validate = await _youtubePopularVideosValidator.ValidateAsync(youtubePopularVideos);
            else if (body is VideoDescriptionRequestModel videoDescription)
                validate = await _videoDescriptionValidator.ValidateAsync(videoDescription);
            else if (body is VideoDescriptionEmailRequestModel videoEmailDescription)
                validate = await _videoDescriptionEmailValidator.ValidateAsync(videoEmailDescription);

            if (!validate.IsValid)
            {
                errorMessage = validate.Errors[0].ErrorMessage;
                await _logger.Log($"Error encountered (parameter error): {errorMessage}");
            }

            return errorMessage;
        }

        private async Task<IResult> CallOpenAIAPI(object body)
        {
            var response = new OpenAIResponseModel { Success = false };

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
                    else if (body is KeywordSearchToolRequestModel keywordSearchTool)
                        result = await _requestData.KeywordSearchTool(keywordSearchTool);
                    else if (body is VideoDescriptionRequestModel videoDescription)
                        result = await _requestData.VideoDescriptionGenerator(videoDescription);
                    else if (body is VideoDescriptionEmailRequestModel videoDescriptionEmail)
                        result = await _requestData.VideoDescriptionEmailGenerator(videoDescriptionEmail);

                    if (result is string res)
                    {
                        response.Message = res;
                        return Results.BadRequest(response);
                    }
                    else
                    {
                        response.Success = true;
                        response.Message = "Success";

                        if (result is VideoDescriptionResponseModel videoDescription)
                            response.Result = videoDescription.Response;
                        else if (result is VideoDescriptionEmailResponseModel videoDescriptionEmail)
                            response.Result = videoDescriptionEmail.Response;
                        else
                            response.Result = result;

                        return Results.Ok(response);
                    }
                }
            }
            catch (Exception e)
            {
                await _logger.Log($"Error encountered: {e.Message}");
                response.Message = e.Message;
                response.Success = false;
                return Results.Json(response, options: null, contentType: null, statusCode: 500);
            }
        }

        public async Task<IResult> GenerateYoutubeTitle(GenerateYoutubeTitleRequestModel body)
        {
            var response = await CallOpenAIAPI(body);
            return response;
        }

        public async Task<IResult> YoutubeChannelFinder(YoutubeChannelFinderRequestModel body)
        {
            try
            {
                var reqParam = await ValidateRequestParameters(body);
                var errorResponse = new YoutubeResponseModel
                {
                    Success = false,
                };

                if (!string.IsNullOrEmpty(reqParam))
                {
                    errorResponse.Message = reqParam;

                    return Results.BadRequest(errorResponse);
                }

                var response = await _requestData.YoutubeChannelFinder(body);

                if (!response.Success)
                    return Results.BadRequest(response);

                return Results.Ok(response);
            }
            catch (Exception e)
            {
                var response = new YoutubeResponseModel
                {
                    Success = false,
                    Message = e.Message
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

                var errorResponse = new YoutubeResponseModel
                {
                    Success = false,
                };

                if (!string.IsNullOrEmpty(reqParam))
                {
                    errorResponse.Message = reqParam;

                    return Results.BadRequest(errorResponse);
                }

                var response = await _requestData.YoutubePopularVideos(body);

                if (!response.Success)
                    return Results.BadRequest(response);

                return Results.Ok(response);
            }
            catch (Exception e)
            {
                var response = new YoutubeResponseModel
                {
                    Success = false,
                    Message = e.Message
                };

                await _logger.Log($"Error encountered: {e.Message}");

                return Results.Json(response, options: null, contentType: null, statusCode: 500);
            }
        }

        public async Task<IResult> HookGenerator(HookGeneratorRequestModel body)
        {
            var response = await CallOpenAIAPI(body);
            return response;
        }

        public async Task<IResult> KeywordSearchTool(KeywordSearchToolRequestModel body)
        {
            var response = await CallOpenAIAPI(body);
            return response;
        }

        public async Task<IResult> VideoDescriptionGenerator(VideoDescriptionRequestModel body)
        {
            var response = await CallOpenAIAPI(body);
            return response;
        }

        public async Task<IResult> VideoDescriptionEmailGenerator(VideoDescriptionEmailRequestModel body)
        {
            var response = await CallOpenAIAPI(body);
            return response;
        }
    }
}