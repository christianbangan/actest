using FluentValidation;

namespace GeneratorAPI.Models.Request.Validators
{
    public class VideoDescriptionEmailRequestModelValidator : AbstractValidator<VideoDescriptionEmailRequestModel>
    {
        public VideoDescriptionEmailRequestModelValidator()
        {
            RuleFor(x => x.Keyword).NotNull().NotEmpty();
            RuleFor(x => x.ChannelName).NotNull().NotEmpty();
            RuleFor(x => x.VideoTitle).NotNull().NotEmpty();
            RuleFor(x => x.BlogTitle).NotNull().NotEmpty();
            RuleFor(x => x.BlogUrl).NotNull().NotEmpty();
        }
    }
}