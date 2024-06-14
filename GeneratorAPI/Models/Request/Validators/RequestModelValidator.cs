using FluentValidation;

namespace GeneratorAPI.Models.Request.Validators
{
    public class RequestModelValidator : AbstractValidator<RequestModel>
    {
        public RequestModelValidator()
        {
            RuleFor(x => x.Keywords).NotNull().WithMessage("");
            RuleFor(x => x.ContentType).NotNull();
        }
    }
}