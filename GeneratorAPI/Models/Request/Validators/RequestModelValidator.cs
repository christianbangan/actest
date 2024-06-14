using FluentValidation;

namespace GeneratorAPI.Models.Request.Validators
{
    public class RequestModelValidator : AbstractValidator<RequestModel>
    {
        public RequestModelValidator()
        {
            RuleFor(x => x.Keywords).NotNull();
            RuleFor(x => x.ContentType).NotNull();
        }
    }
}