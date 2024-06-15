using GeneratorAPI.Services.Interfaces;
using GeneratorAPI.Services;
using GeneratorAPI.Repositories;
using FluentValidation;
using GeneratorAPI.Models.Request;
using GeneratorAPI.Models.Request.Validators;
using GeneratorAPI.Repositories.Interfaces;

namespace GeneratorAPI.ExtensionMethods
{
    public static class Injectors
    {
        public static void Inject(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ILoggerService, LoggerService>();
            builder.Services.AddScoped<IValidator<GenerateYoutubeTitleRequestModel>, GenerateYoutubeTitleRequestModelValidator>();
            builder.Services.AddScoped<IValidator<YoutubeChannelFinderRequestModel>, YoutubeChannelFinderRequestModelValidator>();
            builder.Services.AddScoped<IHttpClientWrapperService, HttpClientWrapperService>();
            builder.Services.AddScoped<IRequestDataService, RequestDataService>();
            builder.Services.AddScoped<IOpenAIRepository, OpenAIRepository>();
        }
    }
}