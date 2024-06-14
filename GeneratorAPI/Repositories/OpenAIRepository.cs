using FluentValidation;
using GeneratorAPI.Models.Request;
using GeneratorAPI.Repositories.Interfaces;
using GeneratorAPI.Services.Interfaces;

namespace GeneratorAPI.Repositories
{
    public class OpenAIRepository(IRequestDataService requestData, IValidator<RequestModel> validator, ILoggerService logger) : IOpenAIRepository
    {
        private readonly IRequestDataService _requestData = requestData;
        private readonly IValidator<RequestModel> _validator = validator;
        private readonly ILoggerService _logger = logger;

        private async Task<string> ValidateRequestParameters(RequestModel body)
        {
            string errorMessage = string.Empty;

            var validate = await _validator.ValidateAsync(body);

            if (!validate.IsValid)
            {
                errorMessage = validate.Errors[0].ErrorMessage;
                await _logger.Log($"Error encountered (parameter error): {errorMessage}");
            }

            return errorMessage;
        }

        public async Task<IResult> GenerateYoutubeTitle(RequestModel body)
        {
            try
            {
                var reqParam = await ValidateRequestParameters(body);

                if (!string.IsNullOrEmpty(reqParam))
                    return Results.BadRequest(reqParam);
                else
                {
                    var response = await _requestData.GenerateYoutubeTitle(body);

                    if (response is string)
                        return Results.BadRequest(response);
                    else
                        return Results.Ok(response);
                }
            }
            catch (Exception e)
            {
                await _logger.Log($"Error encountered: {e.Message}");
                return Results.Json(e.Message, options: null, contentType: null, statusCode: 500);
            }
        }
    }
}