using FluentValidation;
using jConverter.Application.Models;

namespace jConverter.Application.Validators
{
    public class XmlFileValidator : AbstractValidator<InputRequestModel>
    {
        public XmlFileValidator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.ConverterType).NotNull();
            RuleFor(x => x.File).NotNull();
            RuleFor(x => x.FileName).NotNull();
        }
    }
}