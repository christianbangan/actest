using FluentValidation;

namespace GeneratorAPI.Models.Request.Validators
{
    public class VideoDescriptionRequestModelValidator : AbstractValidator<VideoDescriptionRequestModel>
    {
        public VideoDescriptionRequestModelValidator()
        {
            RuleFor(x => x.Keyword).NotNull().NotEmpty();
            RuleFor(x => x.ChannelName).NotNull().NotEmpty();
            RuleFor(x => x.VideoTitle).NotNull().NotEmpty();
            RuleFor(x => x.BlogTitle).NotNull().NotEmpty();
            RuleFor(x => x.BlogUrl).NotNull().NotEmpty();
        }
    }
}