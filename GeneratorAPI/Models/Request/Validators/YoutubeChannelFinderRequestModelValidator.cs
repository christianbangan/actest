using FluentValidation;

namespace GeneratorAPI.Models.Request.Validators
{
    public class YoutubeChannelFinderRequestModelValidator : AbstractValidator<YoutubeChannelFinderRequestModel>
    {
        public YoutubeChannelFinderRequestModelValidator()
        {
            RuleFor(x => x.Query).NotNull().NotEmpty();
        }
    }
}