using FluentValidation;

namespace GeneratorAPI.Models.Request.Validators
{
    public class GenerateYoutubeTitleRequestModelValidator : AbstractValidator<GenerateYoutubeTitleRequestModel>
    {
        public GenerateYoutubeTitleRequestModelValidator()
        {
            RuleFor(x => x.Keywords).NotNull().NotEmpty();
            RuleFor(x => x.ContentType).NotNull().NotEmpty();
        }
    }
}