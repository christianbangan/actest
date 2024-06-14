using GeneratorAPI.Services.Interfaces;
using GeneratorAPI.Services;
using GeneratorAPI.Repositories;

namespace GeneratorAPI.ExtensionMethods
{
    public static class Injectors
    {
        public static void Inject(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ILoggerService, LoggerService>();
            builder.Services.AddScoped<IHttpClientWrapperService, HttpClientWrapperService>();
            builder.Services.AddScoped<IRequestDataService, RequestDataService>();
            builder.Services.AddScoped<IOpenAIRepository, OpenAIRepository>();
            //builder.Services.AddScoped<IG, OpenAIRepository>();
        }
    }
}