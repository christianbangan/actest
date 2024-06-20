using FluentValidation;

namespace GeneratorAPI.Models.Request.Validators
{
    public class YoutubePopularVideosRequestModelValidator : AbstractValidator<YoutubePopularVideosRequestModel>
    {
        public YoutubePopularVideosRequestModelValidator()
        {
            RuleFor(x => x.Keyword).NotNull().NotEmpty();
            RuleFor(x => x.Region).NotNull().NotEmpty();
        }
    }
}