using GeneratorAPI.Services.Interfaces;
using GeneratorAPI.Services;
using GeneratorAPI.Repositories;
using FluentValidation;
using GeneratorAPI.Models.Request;
using GeneratorAPI.Models.Request.Validators;

namespace GeneratorAPI.ExtensionMethods
{
    public static class Injectors
    {
        public static void Inject(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ILoggerService, LoggerService>();
            builder.Services.AddScoped<IValidator<RequestModel>, RequestModelValidator>();
            builder.Services.AddScoped<IHttpClientWrapperService, HttpClientWrapperService>();
            builder.Services.AddScoped<IRequestDataService, RequestDataService>();
            builder.Services.AddScoped<IOpenAIRepository, OpenAIRepository>();
        }
    }
}