using FluentValidation;

namespace GeneratorAPI.Models.Request.Validators
{
    public class HookGeneratorRequestModelValidatorAbstractValidator : AbstractValidator<HookGeneratorRequestModel>
    {
        public HookGeneratorRequestModelValidatorAbstractValidator()
        {
            RuleFor(x => x.Idea).NotNull().NotEmpty();
            RuleFor(x => x.ContentType).NotNull().NotEmpty();
        }
    }
}