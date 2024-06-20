using FluentValidation;

namespace GeneratorAPI.Models.Request.Validators
{
    public class KeywordSearchToolRequestModelValidator : AbstractValidator<KeywordSearchToolRequestModel>
    {
        public KeywordSearchToolRequestModelValidator()
        {
            RuleFor(x => x.Keyword).NotNull().NotEmpty();
        }
    }
}