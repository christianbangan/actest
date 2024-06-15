using FluentValidation;
using GeneratorAPI.Models.Request;
using GeneratorAPI.Models.Response;
using GeneratorAPI.Repositories.Interfaces;
using GeneratorAPI.Services.Interfaces;
using System.Net;

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
            var response = new ResponseModel { StatusCode = (int)HttpStatusCode.BadRequest };

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
                    var result = await _requestData.GenerateYoutubeTitle(body);

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
    }
}